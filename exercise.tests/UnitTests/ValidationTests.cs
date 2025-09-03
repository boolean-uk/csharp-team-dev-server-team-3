using exercise.wwwapi.Helpers;

namespace exercise.tests.UnitTests;
public class ValidationTests
{

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

    [TestCase("username", "Accepted")]
    [TestCase("u", "Accepted")]
    [TestCase("usernameusername", "Accepted")]
    [TestCase("user-name", "Accepted")]
    [TestCase("username1", "Accepted")]
    [TestCase("", "Username must be at least one character")]
    [TestCase("usernameusernameuser", "Username length must be shorter than 17")]
    [TestCase("Username", "Username must only contain lowercase letters 0-9 and -")]
    [TestCase("user_name", "Username must only contain lowercase letters 0-9 and -")]
    [TestCase("!username", "Username must only contain lowercase letters 0-9 and -")]
    [TestCase("invalid@", "Username must only contain lowercase letters 0-9 and -")]
    [TestCase("invalid.", "Username must only contain lowercase letters 0-9 and -")]
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