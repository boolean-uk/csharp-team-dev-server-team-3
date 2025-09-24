﻿using AutoMapper;
using exercise.wwwapi.Configuration;
using exercise.wwwapi.DTOs;
using exercise.wwwapi.DTOs.GetUsers;
using exercise.wwwapi.DTOs.Login;
using exercise.wwwapi.DTOs.Register;
using exercise.wwwapi.Helpers;
using exercise.wwwapi.Models;
using exercise.wwwapi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace exercise.wwwapi.EndPoints
{
    public static class UserEndpoints
    {
        public static void ConfigureAuthApi(this WebApplication app)
        {
            app.MapPost("/login", Login).WithSummary("Localhost Login");

            var users = app.MapGroup("users");
            users.MapPost("/", Register).WithSummary("Create user");
            users.MapGet("/", GetUsers).WithSummary("Get all users by first name if provided");
            users.MapGet("/{id:int}", GetUserById).WithSummary("Get user by user id");
            users.MapPatch("/{id:int}", UpdateUser).WithSummary("Update a user");

        }

        /// <summary>
        /// Retrieves users, optionally filtered by a case-insensitive search on first name, last name, or full name.
        /// </summary>
        /// <param name="repository">
        /// The user repository used to fetch users.
        /// </param>
        /// <param name="claims">
        /// <see cref="ClaimsPrincipal"/>-user that authorizes the user to use this endpoint.
        /// </param>
        /// <param name="name">
        /// Optional search term to filter users by first name, last name, or "FirstName LastName".
        /// </param>
        /// <returns>
        /// <see cref="IResult"/> with a 200 OK containing a <see cref="ResponseDTO{T}"/> of <see cref="UsersSuccessDTO"/> on success,
        /// or a 401 Unauthorized if the caller is not authorized.
        /// </returns>
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        private static async Task<IResult> GetUsers(IRepository<User> repository, ClaimsPrincipal claims, string? name)
        {
            int? id = claims.UserRealId();

            IEnumerable<User> results = await repository.Get();
            string? search = name?.Trim().ToLower();

            UsersSuccessDTO userData = new UsersSuccessDTO()
            {
                Users = !string.IsNullOrWhiteSpace(search)
                    ? results.Where(i =>
                        (i.FirstName.Contains(search, StringComparison.CurrentCultureIgnoreCase)) ||
                        (i.LastName.Contains(search, StringComparison.CurrentCultureIgnoreCase)) ||
                        ($"{i.FirstName ?? ""} {i.LastName ?? ""}".Contains(search, StringComparison.CurrentCultureIgnoreCase))
                    ).ToList()
                    : results.ToList()
            };

            ResponseDTO<UsersSuccessDTO> response = new ResponseDTO<UsersSuccessDTO>()
            {
                Message = "success",
                Data = userData
            };

            return TypedResults.Ok(response);
        }

        // TODO: Docstring
        // Public endpoint, not authorized
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        private static IResult Register(IRepository<User> repository, IMapper mapper, RegisterRequestDTO request)
        {
            // Validate password
            string validationResult = Validator.Password(request.password);
            if (validationResult != "Accepted") return TypedResults.BadRequest(new ResponseDTO<string>() { Message = validationResult });

            // Validate email
            string emailValidation = Validator.Email(request.email);
            if (emailValidation != "Accepted") return TypedResults.BadRequest(new ResponseDTO<string>() { Message = emailValidation });
            var emailExists = repository.GetAllFiltered(q => q.Email == request.email);
            if (emailExists.Any()) return Results.Conflict(new ResponseDTO<string>() { Message = "Fail" });

            // Put user
            var user = new User
            {
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.password),
                Email = request.email
            };
            repository.Insert(user);
            repository.Save();

            ResponseDTO<UserDTO> response = new ResponseDTO<UserDTO>
            {
                Message = "success",
                Data = mapper.Map<UserDTO>(user)
            };

            return Results.Created($"/users/{user.Id}", response);
        }

        // TODO: Docstring
        // Public endpoint (not authorized)
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        private static IResult Login(IRepository<User> repository, IMapper mapper, LoginRequestDTO request, IConfigurationSettings config)
        {
            //if (string.IsNullOrEmpty(request.username)) request.username = request.email;

            // HAVE to check for valid email/password before verifying if the user already exists, otherwise breach of security

            // Check for valid password
            string validationResult = Validator.Password(request.password);
            Console.WriteLine($"Password: {validationResult}");
            if (validationResult != "Accepted") return TypedResults.BadRequest(new ResponseDTO<string>() { Message = "Invalid email and/or password provided" });


            //email doesn't exist, should probably be 404 user not found, but should maybe just say invalid email or password
            //check if email is in database
            var emailExists = repository.GetAllFiltered(q => q.Email == request.email);
            if (!emailExists.Any()) return TypedResults.BadRequest(new ResponseDTO<string>() { Message = "Invalid email and/or password provided" });

            User user = repository.GetAll().FirstOrDefault(u => u.Email == request.email)!;


            if (!BCrypt.Net.BCrypt.Verify(request.password, user.PasswordHash))
            {
                // should probably be 401 unauthorized
                return Results.BadRequest(new ResponseDTO<Object>() { Message = "Invalid email and/or password provided" });
            }

            string token;
            if (request.longlifetoken.GetValueOrDefault()) token = CreateToken(user, config, 7);
            else token = CreateToken(user, config, 1.0 / 24);


            ResponseDTO<LoginSuccessDTO> response = new ResponseDTO<LoginSuccessDTO>
            {
                Message = "success",
                Data = new LoginSuccessDTO()
                {
                    // Maps user to UserDTO
                    User = mapper.Map<UserDTO>(user),
                    Token = token
                }
            };

            return Results.Ok(response);

        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        private static async Task<IResult> GetUserById(IRepository<User> repository, ClaimsPrincipal claims, IMapper mapper, int id)
        {
            var user = repository.GetById(id);
            if (user == null) return TypedResults.NotFound(new ResponseDTO<string> { Message = "User not found" });

            ResponseDTO<UserDTO> response = new ResponseDTO<UserDTO>
            {
                Message = "success",
                Data = mapper.Map<UserDTO>(user)
            };

            return TypedResults.Ok(response);
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        private static async Task<IResult> UpdateUser(IRepository<User> repository, ClaimsPrincipal claims, IMapper mapper, int id, UserPatchDTO userPatch)
        {
            if (userPatch.GetType().GetProperties().Length > 0 && userPatch.GetType().GetProperties().All((p) => p.GetValue(userPatch) == null))
                return TypedResults.BadRequest(new ResponseDTO<string>() { Message = "Provide at least one field for update" });

            var user = repository.GetById(id);

            if (user == null) return TypedResults.NotFound(new ResponseDTO<string> { Message = "User not found" });

            if (user.Id != claims.UserRealId() && claims.Role() == (int)Roles.student)
            {
                var forbiddenResponse = new ResponseDTO<object>
                {
                    Message = "You are not authorized to edit this post."
                };
                return TypedResults.Json(forbiddenResponse, statusCode: StatusCodes.Status403Forbidden);
            }

            if (userPatch.Username != null && userPatch.Username != user.Username)
            {
                // Validate username
                if (Validator.Username(userPatch.Username) != "Accepted") return TypedResults.BadRequest(new ResponseDTO<string>() { Message = "Invalid username" });
                var usernameExists = repository.GetAllFiltered(q => q.Username == userPatch.Username);
                if (usernameExists.Any()) return TypedResults.BadRequest(new ResponseDTO<string>() { Message = "Username is already in use" });
                // Update
                user.Username = userPatch.Username;
            }
            if (userPatch.GithubUsername != null && userPatch.GithubUsername != user.GithubUsername)
            {
                // Validate github username
                if (Validator.Username(userPatch.GithubUsername) != "Accepted") return TypedResults.BadRequest(new ResponseDTO<string>() { Message = "Invalid GitHub username" });
                var gitUsernameExists = repository.GetAllFiltered(q => q.GithubUsername == userPatch.GithubUsername);
                if (gitUsernameExists.Any()) return TypedResults.BadRequest(new ResponseDTO<string>() { Message = "GitHub username is already in use" });
                // Update
                user.GithubUsername = userPatch.GithubUsername;
            }
            if (userPatch.Email != null && userPatch.Email != user.Email)
            {
                // Validate email
                if (Validator.Email(userPatch.Email) != "Accepted") return TypedResults.BadRequest(new ResponseDTO<string>() { Message = "Invalid email" });
                var emailExists = repository.GetAllFiltered(q => q.Email == userPatch.Email);
                if (emailExists.Any()) return TypedResults.BadRequest(new ResponseDTO<string>() { Message = "Email is already in use" });
                // Update
                user.Email = userPatch.Email;
            }
            if (userPatch.Password != null)
            {
                // Validate username
                if (Validator.Password(userPatch.Password) != "Accepted") return TypedResults.BadRequest(new ResponseDTO<string>() { Message = "Invalid password" });
                // Hash
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(userPatch.Username);
                // Update
                user.PasswordHash = passwordHash;
            }
            if (userPatch.FirstName != null) user.FirstName = userPatch.FirstName;
            if (userPatch.LastName != null) user.LastName = userPatch.LastName;
            if (userPatch.Mobile != null) user.Mobile = userPatch.Mobile;
            if (userPatch.Role != null)
            {
                if (userPatch.Role == 0) { user.Role = Roles.student; }
                else if (userPatch.Role == 1) { user.Role = Roles.teacher; }
                else { return TypedResults.BadRequest(new ResponseDTO<string>() { Message = "Role does not exist" }); }
            }
            if (userPatch.Specialism != null) user.Specialism = userPatch.Specialism;
            // TODO: Add cohort support after implementing the Cohort model and adding it to user.
            //if (userPatch.Cohort != null) user.Cohort = userPatch.Cohort;
            if (userPatch.StartDate != null) user.StartDate = (DateTime)userPatch.StartDate;
            if (userPatch.EndDate != null) user.EndDate = (DateTime)userPatch.EndDate;
            if (userPatch.Bio != null) user.Bio = userPatch.Bio;
            if (userPatch.Photo != null) user.Photo = userPatch.Photo;

            repository.Update(user);
            repository.Save();

            ResponseDTO<UserDTO> response = new ResponseDTO<UserDTO>
            {
                Message = "success",
                Data = mapper.Map<UserDTO>(user)
            };

            return TypedResults.Ok(response);
        }

        // Helper, creates jwt tokens
        private static string CreateToken(User user, IConfigurationSettings config, double days)
        {
            List<Claim> claims =
            [
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, ((int)user.Role).ToString())
            ];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetValue("AppSettings:Token")));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(days),
                signingCredentials: credentials
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}

