using exercise.wwwapi.Helpers;

namespace exercise.tests.UnitTests;

/// <summary>
/// Unit tests for the static <see cref="Validator"/> helpers that gatekeep input format rules.
/// </summary>
public class ValidationTests
{

    /// <summary>
    /// Ensures <see cref="Validator.Password(string)"/> returns the expected message for representative passwords.
    /// </summary>
    /// <param name="input">The password candidate being evaluated.</param>
    /// <param name="expected">The validator response that should be produced.</param>
    [TestCase("Valid123!", "Accepted")]
    [TestCase("short1!", "Too few characters")]
    [TestCase("noupper123!", "Missing uppercase characters")]
    [TestCase("NoNumber!", "Missing number(s) in password")]
    [TestCase("NoSpecial1", "Missing special character")]
    [TestCase("V3rySp3ci&l", "Accepted")]
    public void ValidatePassword(string input, string expected)
    {
        // act 
        // no setup needed as Validator class is static

        // arrange
        string result = Validator.Password(input);

        // assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Verifies <see cref="Validator.Email(string)"/> distinguishes between valid and invalid email formats.
    /// </summary>
    /// <param name="input">The email address under validation.</param>
    /// <param name="expected">The expected validation outcome message.</param>
    [TestCase("valid@email.com", "Accepted")]
    [TestCase("valid@email.com.no", "Accepted")]
    [TestCase("valid.mail@email.com", "Accepted")]
    [TestCase("invalid.com", "Invalid email format")]
    [TestCase("invalid@", "Invalid email format")]
    [TestCase("invalid", "Invalid email format")]
    [TestCase("invalid@..no", "Invalid email domain")]
    [TestCase("invalid@text", "Invalid email domain")]
    [TestCase("invalid@.email.com", "Invalid email domain")]
    [TestCase("invalid@email.com.", "Invalid email domain")]
    public void ValidateEmail(string input, string expected)
    {
        // act 
        // no setup needed as Validator class is static

        // arrange
        string result = Validator.Email(input);

        // assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Confirms <see cref="Validator.Username(string)"/> accepts only usernames matching the allowed pattern.
    /// </summary>
    /// <param name="input">The username candidate to validate.</param>
    /// <param name="expected">The descriptive result the validator should return.</param>
    [TestCase("username", "Accepted")]
    [TestCase("user-name", "Accepted")]
    [TestCase("user-name-valid", "Accepted")]
    [TestCase("username1", "Accepted")]
    [TestCase("user1name", "Accepted")]
    [TestCase("1user-name", "Accepted")]
    [TestCase("user-name-1", "Accepted")]
    [TestCase("u", "Accepted")]
    [TestCase("1", "Accepted")]
    [TestCase("", "Length of username must be at least 1.")]
    [TestCase("usernameusernameusernameusernameusernameusername", "Length of username must be at most 39.")]
    [TestCase("-username", "Username must contain only alphanumeric characters that may be separated by single hyphens")]
    [TestCase("username-", "Username must contain only alphanumeric characters that may be separated by single hyphens")]
    [TestCase("-username-", "Username must contain only alphanumeric characters that may be separated by single hyphens")]
    [TestCase("-", "Username must contain only alphanumeric characters that may be separated by single hyphens")]
    [TestCase("user--name", "Username must contain only alphanumeric characters that may be separated by single hyphens")]
    [TestCase("user--name-invalid", "Username must contain only alphanumeric characters that may be separated by single hyphens")]
    [TestCase("username!", "Username must contain only alphanumeric characters that may be separated by single hyphens")]
    [TestCase("username?", "Username must contain only alphanumeric characters that may be separated by single hyphens")]
    [TestCase("username+", "Username must contain only alphanumeric characters that may be separated by single hyphens")]
    [TestCase("username`", "Username must contain only alphanumeric characters that may be separated by single hyphens")]
    [TestCase("username´", "Username must contain only alphanumeric characters that may be separated by single hyphens")]
    [TestCase("username/", "Username must contain only alphanumeric characters that may be separated by single hyphens")]
    [TestCase("username%", "Username must contain only alphanumeric characters that may be separated by single hyphens")]
    [TestCase("username_", "Username must contain only alphanumeric characters that may be separated by single hyphens")]
    [TestCase("user name", "Username must contain only alphanumeric characters that may be separated by single hyphens")]
    [TestCase("øsername", "Username must contain only alphanumeric characters that may be separated by single hyphens")]
    public void ValidateUsername(string input, string expected)
    {
        // act 
        // no setup needed as Validator class is static

        // arrange
        string result = Validator.Username(input);

        // assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(expected));
    }
}
