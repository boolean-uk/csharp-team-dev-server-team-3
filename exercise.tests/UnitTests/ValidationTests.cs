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
}