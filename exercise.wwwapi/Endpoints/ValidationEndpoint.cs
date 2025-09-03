﻿
using exercise.wwwapi.DTOs.Register;
using exercise.wwwapi.DTOs.Validation;
using exercise.wwwapi.Helpers;
using exercise.wwwapi.Models;
using exercise.wwwapi.Repository;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace exercise.wwwapi.Endpoints
{
    public static class ValidationEndpoint
    {
        public static void ConfigureValidationEndpoint(this WebApplication app)
        {
            var validatiors = app.MapGroup("/validation");
            validatiors.MapPost("/password", ValidatePassword).WithSummary("Validate a password");
            validatiors.MapGet("/username/{username}", ValidateUsername).WithSummary("Validate a Username");
            validatiors.MapGet("/email/{email}", ValidateEmail).WithSummary("Validate an email address");

        }

        /// <summary>
        /// Validates an email using custom email rules.
        /// </summary>
        /// <param name="repository"> A <see cref="IRepository{User}"/> object used to query the user data source for existing emails.</param>
        /// <param name="email">The email string to validate.</param>
        /// <returns>
        /// 200 OK response with a message if the password is accepted.<br/>
        /// 400 Bad Request with a message if the email is invalid or if email already exists in database.
        /// </returns>
        /// <response code="200">Email is valid and accepted</response>
        /// <response code="400">Email is invalid or already exists</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        private static IResult ValidateEmail(IRepository<User> repository, string email)
        {
            if (email == null || string.IsNullOrEmpty(email)) return TypedResults.BadRequest("Something went wrong!");
            string result = Helpers.Validator.Email(email);
            if (result != "Accepted") return TypedResults.BadRequest(result);
            var emailExists = repository.GetAllFiltered(q => q.Email == email);
            if (emailExists.Count() != 0) return TypedResults.BadRequest("Email already exists");
            return TypedResults.Ok(result);
        }
  
        /// <summary>
        /// Validates a password using custom password rules.
        /// </summary>
        /// <param name="password">A <see cref="PasswordDTO"/> object containing the password to validate.</param>
        /// <returns>
        /// 200 OK response if the password is accepted.<br/>
        /// 400 Bad Request with a message if the password is invalid or if validation fails. 
        /// </returns>
        /// <response code="200">Password is valid and accepted.</response>
        /// <response code="400">Password is invalid or validation failed.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        private static IResult ValidatePassword(PasswordDTO passwordDTO)
        {
            if (passwordDTO == null || string.IsNullOrEmpty(passwordDTO.password))
                return TypedResults.BadRequest("Something went wrong!");
            string result = Helpers.Validator.Password(passwordDTO.password);
            if (result == null) return TypedResults.BadRequest("Something went wrong!");
            else if (result == "Accepted") return TypedResults.Ok(result);
            else return TypedResults.BadRequest(result);
        }

        /// <summary>
        /// Validates a username using custom username rules.
        /// Checks if username is already in database, bad request if it exists<br/>
        /// </summary>
        /// <param name="username">A <see cref="UsernameDTO"/> object containing the username to validate.</param>
        /// 
        /// <returns>
        /// 200 OK response if the username is accepted.<br/>
        /// 400 Bad Request with a message if the username is invalid or if validation fails. 
        /// </returns>
        /// <response code="200">Username is valid and accepted.</response>
        /// <response code="400">Username is invalid or validation failed.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        private static IResult ValidateUsername(IRepository<User> repository, string username)
        {
            if (username == null || string.IsNullOrEmpty(username))
                return TypedResults.BadRequest("Empty input");
            string result = Helpers.Validator.Username(username);
            if (result == null) return TypedResults.BadRequest("Empty response from server");
            var usernameExists = repository.GetAllFiltered(q => q.Username == username);
            if (usernameExists.Count() != 0) return TypedResults.BadRequest("Username is already in use");
            if (result == "Accepted") return TypedResults.Ok(result);
            return TypedResults.BadRequest(result);
        }
    }
}
