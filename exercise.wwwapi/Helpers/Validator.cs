using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.ComponentModel.DataAnnotations;

namespace exercise.wwwapi.Helpers
{
    public static class Validator
    {

        /// <summary>
        /// Validates a password string against a set of security rules.<br/>
        /// - Minimum length of 8 characters. <br/>
        /// - At least one uppercase letter.<br/>
        /// - At least one numeric digit.<br/>
        /// - At least one special character from the set: !@#$%^&amp;*()-_=+{};:',./\|~
        /// </summary>
        /// <param name="passwordString">The password string to validate.</param>
        /// <returns>
        /// A string indicating the result of the validation:<br/>
        /// - "Accepted" if the password meets all criteria.<br/>
        /// - A descriptive error message if any rule is violated.
        /// </returns>
        public static string Password(string passwordString)
        {
            // Not less than 8 characters       | Done
            // Atleast one uppercase            | Done
            // Atleast one number               | Done
            // Atleast one special character    | Done

            if (passwordString.Count() < 8) return "Too few characters";

            if (!passwordString.Any(char.IsUpper)) return "Missing uppercase characters";

            if (!passwordString.Any(char.IsNumber)) return "Missing number(s) in password";

            // Accept only !@#$%^&*()-_=+{};:',./\|~
            string regexPattern = ".*[!@#$%^&*()\\-_=+{};:',./\\\\|~].*";
            if (!Regex.IsMatch(passwordString, regexPattern)) return "Missing special character";
            return "Accepted";
        }


        /// <summary>
        /// Validates a username string against a set of security rules.<br/>
        /// - Minimum length of 1 characters. <br/>
        /// - maximum length of 1 characters.<br/>
        /// - Only a-z 0-9 and - allowed.
        /// </summary>
        /// <param name="usernameString">The username string to validate.</param>
        /// <returns>
        /// A string indicating the result of the validation:<br/>
        /// - "Accepted" if the password meets all criteria.<br/>
        /// - A descriptive error message if any rule is violated.
        /// </returns>
        public static string Username(string usernameString)
        {

            if (usernameString.Count() > 17) return "Username length must be shorter than 17";
            if (usernameString.Count() < 1) return "Username must be at least one character";

            string regexPattern = "^[a-z0-9-]+$";
            if (!Regex.IsMatch(usernameString, regexPattern)) return "Username must only contain lowercase letters 0-9 and -";

            return "Accepted";
        }

          
        public static string Email(string emailString)
        {
            if (!new EmailAddressAttribute().IsValid(emailString)) return "Invalid email format";
            if (!Regex.IsMatch(emailString, @"@([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}$")) return "Invalid email domain";
            return "Accepted";

        }

        /// <summary>
        /// Validates a GitHub username string against a set of rules.<br/>
        /// - Minimum length of 39 characters. <br/>
        /// - maximum length of 1 characters.<br/>
        /// - Only alphanumeric characters and non-consecutive hyphens allowed.
        /// </summary>
        /// <param name="usernameString">The username string to validate.</param>
        /// <returns>
        /// A string indicating the result of the validation:<br/>
        /// - "Accepted" if the username is valid.<br/>
        /// - A descriptive error message if any rule is violated.
        /// </returns>
        public static string GitUsername(string usernameString)
        {
            if (string.IsNullOrWhiteSpace(usernameString)) return "Username cannot be empty";
            if (usernameString.Count() < 1) return "Length of username must be at least 1.";
            if (usernameString.Count() > 39) return "Length of username must be at most 39.";

            string regexPattern = "^(?!.*--)(?!-)(?!.*-$)[a-zA-Z0-9]+(-[a-zA-Z0-9]+)*$";
            if (!Regex.IsMatch(usernameString, regexPattern)) return "Username must contain only alphanumeric characters that may be separated by single hyphens";

            return "Accepted";
        }

    }
}
