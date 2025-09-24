﻿
using exercise.wwwapi.DTOs;
using exercise.wwwapi.DTOs.Validation;
using exercise.wwwapi.Models;
using exercise.wwwapi.Repository;
using Microsoft.AspNetCore.Mvc;

namespace exercise.wwwapi.Endpoints
{
    public static class ValidationEndpoint
    {
        public static void ConfigureValidationEndpoint(this WebApplication app)
        {
            var validators = app.MapGroup("/validation");
            validators.MapPost("/password", ValidatePassword).WithSummary("Validate a password");
            validators.MapGet("/email/{email}", ValidateEmail).WithSummary("Validate an email address");
            validators.MapGet("/username", ValidateUsername).WithSummary("Validate a username");
            validators.MapGet("/git-username", ValidateGitUsername).WithSummary("Validate a GitHub username");
        }

        /// <summary>
        /// Validates a username based GitHub's rules.
        /// Source: https://docs.github.com/en/enterprise-cloud@latest/admin/managing-iam/iam-configuration-reference/username-considerations-for-external-authentication
        /// </summary>
        /// <param name="repository"> A <see cref="IRepository{User}"/> object used to query the user data source for existing GitHub usernames.</param>
        /// <param name="username">The username string to validate.</param>
        /// <returns>
        /// 200 OK response with a message if the username is accepted.<br/>
        /// 400 Bad Request with a message if the username is invalid or if username already exists in database.
        /// </returns>
        /// <response code="200">Email is valid and accepted</response>
        /// <response code="400">Email is invalid or already exists</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        private static IResult ValidateGitUsername(IRepository<User> repository, [FromQuery] string username)
        {
            string result = Helpers.Validator.Username(username);
            if (result != "Accepted") return TypedResults.BadRequest(new ResponseDTO<Object>() { Message = result });

            var gitUsernameExists = repository.GetAllFiltered(q => q.GithubUsername == username);
            if (gitUsernameExists.Any()) return TypedResults.BadRequest(new ResponseDTO<Object>() { Message = "GitHub username is already in use" });

            return TypedResults.Ok(new ResponseDTO<Object>() { Message = result });
        }

        /// <summary>
        /// Validates an email using custom email rules.
        /// </summary>
        /// <param name="repository"> A <see cref="IRepository{User}"/> object used to query the user data source for existing emails.</param>
        /// <param name="email">The email string to validate.</param>
        /// <returns>
        /// 200 OK response with a message if the email is accepted.<br/>
        /// 400 Bad Request with a message if the email is invalid or if email already exists in database.
        /// </returns>
        /// <response code="200">Email is valid and accepted</response>
        /// <response code="400">Email is invalid or already exists</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        private static IResult ValidateEmail(IRepository<User> repository, string email)
        {
            string result = Helpers.Validator.Email(email);
            if (result != "Accepted") return TypedResults.BadRequest(new ResponseDTO<Object>() { Message = result });

            var emailExists = repository.GetAllFiltered(q => q.Email == email);
            if (emailExists.Any()) return TypedResults.BadRequest(new ResponseDTO<Object>() { Message = "Email already exists" });

            return TypedResults.Ok(new ResponseDTO<Object>() { Message = result });
        }

        /// <summary>
        /// Validates a password using custom password rules.
        /// </summary>
        /// <param name="passwordDto">A <see cref="passwordDto"/> object containing the password to validate.</param>
        /// <returns>
        /// 200 OK response if the password is accepted.<br/>
        /// 400 Bad Request with a message if the password is invalid or if validation fails. 
        /// </returns>
        /// <response code="200">Password is valid and accepted.</response>
        /// <response code="400">Password is invalid or validation failed.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        private static IResult ValidatePassword(PasswordDTO? passwordDto)
        {
            if (passwordDto == null || string.IsNullOrEmpty(passwordDto.password)) return TypedResults.BadRequest(new ResponseDTO<Object>() { Message = "Something went wrong!" });

            string result = Helpers.Validator.Password(passwordDto.password);
            if (result == "Accepted") return TypedResults.Ok(new ResponseDTO<Object>() { Message = result });

            return TypedResults.BadRequest(new ResponseDTO<Object>() { Message = result });
        }

        /// <summary>
        /// Validates a username using custom username rules.
        /// Checks if username is already in database, bad request if it exists<br/>
        /// </summary>
        /// <param name="repository"> A <see cref="IRepository{User}"/> object used to query the user data source for existing usernames.</param>
        /// <param name="username">A string containing the username to validate.</param>
        /// 
        /// <returns>
        /// 200 OK response if the username is accepted.<br/>
        /// 400 Bad Request with a message if the username is invalid or if validation fails. 
        /// </returns>
        /// <response code="200">Username is valid and accepted.</response>
        /// <response code="400">Username is invalid or validation failed.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        private static IResult ValidateUsername(IRepository<User> repository, [FromQuery] string username)
        {
            string result = Helpers.Validator.Username(username);
            if (result != "Accepted") return TypedResults.BadRequest(new ResponseDTO<Object>() { Message = result });

            var usernameExists = repository.GetAllFiltered(q => q.Username == username);
            if (usernameExists.Any()) return TypedResults.BadRequest(new ResponseDTO<Object>() { Message = "Username is already in use" });

            return TypedResults.Ok(new ResponseDTO<Object>() { Message = result });
        }
    }
}
