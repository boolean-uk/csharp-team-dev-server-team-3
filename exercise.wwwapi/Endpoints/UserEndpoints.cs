using AutoMapper;
using exercise.wwwapi.Configuration;
using exercise.wwwapi.DTOs;
using exercise.wwwapi.DTOs.GetUsers;
using exercise.wwwapi.DTOs.Login;
using exercise.wwwapi.DTOs.Register;
using exercise.wwwapi.Helpers;
using exercise.wwwapi.Models;
using exercise.wwwapi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace exercise.wwwapi.EndPoints
{
    public static class UserEndpoints
    {
        public static void ConfigureAuthApi(this WebApplication app)
        {
            var users = app.MapGroup("users");
            users.MapPost("/", Register).WithSummary("Create user");
            users.MapGet("/", GetUsers).WithSummary("Get all users by first name if provided");
            users.MapGet("/{id}", GetUserById).WithSummary("Get user by user id");
            app.MapPost("/login", Login).WithSummary("Localhost Login");
            app.MapPatch("/{id}", UpdateUser).WithSummary("Update a user");
        }
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        private static async Task<IResult> GetUsers(IRepository<User> service, string? firstName, ClaimsPrincipal user)
        {
            IEnumerable<User> results = await service.Get();
            UsersSuccessDTO userData = new UsersSuccessDTO() { 
                Users = !string.IsNullOrEmpty(firstName) 
                    ? results.Where(i => i.Email.Contains(firstName)).ToList() 
                    : results.ToList() };
            ResponseDTO<UsersSuccessDTO> response = new ResponseDTO<UsersSuccessDTO>() 
                {
                    Status = "success", 
                    Data = userData 
                };
            return TypedResults.Ok(response);
        }

        // should 200OK be 201Created?
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        private static IResult Register(RegisterRequestDTO request, IRepository<User> service, IMapper mapper)
        {
            //user exists
            if (service.GetAll().Where(u => u.Email == request.email).Any()) return Results.Conflict(new ResponseDTO<RegisterFailureDTO>() { Status = "Fail" });

            // check valid password
            string validationResult = Validator.Password(request.password);
            if (validationResult != "Accepted") return TypedResults.BadRequest(new ResponseDTO<RegisterFailureDTO>() { Status = validationResult });

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.password);

            var user = new User();

            user.Username = !string.IsNullOrEmpty(request.username) ? request.username : request.email;
            user.PasswordHash = passwordHash;
            user.Email = request.email; 
            user.FirstName = !string.IsNullOrEmpty(request.firstName) ? request.firstName : string.Empty;
            user.LastName = !string.IsNullOrEmpty(request.lastName) ? request.lastName : string.Empty;
            user.Bio = !string.IsNullOrEmpty(request.bio) ? request.bio : string.Empty;
            user.GithubUrl = !string.IsNullOrEmpty(request.githubUrl) ? request.githubUrl : string.Empty;

            service.Insert(user);
            service.Save();

            ResponseDTO<UserDTO> response = new ResponseDTO<UserDTO>
            {
                Status = "success",
                Data = mapper.Map<UserDTO>(user)
            };

            return Results.Ok(response);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        private static IResult Login(LoginRequestDTO request, IRepository<User> service, IConfigurationSettings config, IMapper mapper)
        {
            //if (string.IsNullOrEmpty(request.username)) request.username = request.email;

            // HAVE to check for valid email/password before verifying if the user already exists, otherwise breach of security
            
            // Check for valid password
            string validationResult = Validator.Password(request.password);
            if (validationResult != "Accepted") return TypedResults.BadRequest(new ResponseDTO<string>() { Status = "Fail", Data = "Invalid email and/or password provided" });


            //user doesn't exist, should probably be 404 user not found, but should maybe just say invalid email or password
            if (!service.GetAll().Where(u => u.Email == request.email).Any()) return Results.BadRequest(new Payload<Object>() { status = $"{request.email} does not exist", data = new { email="Invalid email and/or password provided"} });

            User user = service.GetAll().FirstOrDefault(u => u.Email == request.email)!;
           
            
            if (!BCrypt.Net.BCrypt.Verify(request.password, user.PasswordHash))
            {
                // should probably be 401 unauthorized
                return Results.BadRequest(new Payload<Object>() { status = "fail", data = new LoginFailureDTO() });
            }

            string token = CreateToken(user, config);

            ResponseDTO<LoginSuccessDTO> response = new ResponseDTO<LoginSuccessDTO>
            {
                Status = "success",
                Data = new LoginSuccessDTO()
                {
                    // Maps user to UserDTO
                    User = mapper.Map<UserDTO>(user),
                    Token = token
                }
            };

            return Results.Ok(response) ;
           
        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        public static async Task<IResult> GetUserById(int id)
        {
            return TypedResults.Ok();
        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        public static async Task<IResult> UpdateUser(int id)
        {
            return TypedResults.Ok();
        }
        private static string CreateToken(User user, IConfigurationSettings config)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetValue("AppSettings:Token")));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}

