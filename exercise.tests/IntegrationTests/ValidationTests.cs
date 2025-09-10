using exercise.tests.Helpers;
using exercise.wwwapi.DTOs.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace exercise.tests.IntegrationTests
{
    public class ValidationTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [SetUp]
        public void SetUp()
        {
            // Arrange 
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [TestCase("Valid123!",  HttpStatusCode.OK)]
        [TestCase("short1!",  HttpStatusCode.BadRequest)]
        [TestCase("noupper123!",  HttpStatusCode.BadRequest)]
        [TestCase("NoNumber!",  HttpStatusCode.BadRequest)]
        [TestCase("NoSpecial1",  HttpStatusCode.BadRequest)]
        [TestCase("V3rySp3ci&l", HttpStatusCode.OK)]
        [TestCase("", HttpStatusCode.BadRequest)]
        [TestCase(null!, HttpStatusCode.BadRequest)]
        public async Task ValidatePasswordStatus(string? input, HttpStatusCode statusCode)
        {
            // Arrange
            PasswordDTO body = new PasswordDTO { password = input };
            var json = JsonSerializer.Serialize(body);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/validation/password", requestBody);


            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(statusCode));
        }

        //[TestCase(5, "Something went wrong!")]
        [TestCase("Valid123!", "Accepted")]
        [TestCase("short1!", "Too few characters")]
        [TestCase("noupper123!", "Missing uppercase characters")]
        [TestCase("NoNumber!", "Missing number(s) in password")]
        [TestCase("NoSpecial1", "Missing special character")]
        [TestCase("V3rySp3ci&l", "Accepted")]
        [TestCase("", "Something went wrong!")]
        [TestCase(null!, "Something went wrong!")]
        public async Task ValidatePasswordMessage(string? input, string expected)
        {
            // Arrange
            PasswordDTO body = new PasswordDTO { password = input };
            var json = JsonSerializer.Serialize(body);
            Console.WriteLine(json.ToString());
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/validation/password", requestBody);
            Console.WriteLine("r,",response);

            // Assert
            var contentString = await response.Content.ReadAsStringAsync();

            JsonNode? message = null;
            if (!string.IsNullOrWhiteSpace(contentString))
            {
                message = JsonNode.Parse(contentString);
            }

            Console.WriteLine("Message: " + message);
            Assert.That(message, Is.Not.Null);
            Assert.That(message?["message"]?.ToString(), Is.EqualTo(expected));
        }

        [TestCase("valid@email.com", HttpStatusCode.OK)]
        [TestCase("valid@email.com.no", HttpStatusCode.OK)]
        [TestCase("valid.mail@email.com", HttpStatusCode.OK)]
        [TestCase("invalid.com", HttpStatusCode.BadRequest)]
        [TestCase("invalid@", HttpStatusCode.BadRequest)]
        [TestCase("invalid", HttpStatusCode.BadRequest)]
        [TestCase("invalid@..no", HttpStatusCode.BadRequest)]
        [TestCase("invalid@text", HttpStatusCode.BadRequest)]
        [TestCase("invalid@.email.com", HttpStatusCode.BadRequest)]
        [TestCase("invalid@email.com.", HttpStatusCode.BadRequest)]
        public async Task ValidateEmailStatus(string input, HttpStatusCode statusCode)
        {
            // Arrange

            // Act
            var response = await _client.GetAsync($"/validation/email/{input}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(statusCode));
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
        public async Task ValidateEmailMessage(string input, string expected)
        {
            // Arrange

            // Act
            var response = await _client.GetAsync($"/validation/email/{input}");

            // Assert
            var contentString = await response.Content.ReadAsStringAsync();

            JsonNode? message = null;
            if (!string.IsNullOrWhiteSpace(contentString))
            {
                message = JsonNode.Parse(contentString);
            }

            Console.WriteLine("Message: " + message);
            Assert.That(message, Is.Not.Null);
            Assert.That(message?["message"]?.ToString(), Is.EqualTo(expected));
        }


        /// <summary>
        /// Testing username validation. Asserting that response status codes are correct.
        /// Test cases are defined in <see cref="UsernameValidationTestData"/>.
        /// </summary>
        /// <param name="endpoint"> The endpoint. All cases run with endpoints "username" and "git-username".</param>
        /// <param name="input"> The provided input.</param>
        /// <param name="expected"> The expected response status code.</param>
        /// <returns></returns>
        [Test, TestCaseSource(typeof(UsernameValidationTestData), nameof(UsernameValidationTestData.UsernameValidationStatusCases))]
        public async Task ValidateUsernameStatus(string endpoint, string input, HttpStatusCode expected)
        {
            // Arrange

            // Act
            var response = await _client.GetAsync($"/validation/{endpoint}?username={input}");
            Console.WriteLine($"{expected} : {response.StatusCode}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(expected));
        }


        /// <summary>
        /// Testing username validation. Asserting that response messages are correct.
        /// Test cases are defined in <see cref="UsernameValidationTestData"/>.
        /// </summary>
        /// <param name="endpoint"> The endpoint. All cases run with endpoints "username" and "git-username".</param>
        /// <param name="input"> The provided input.</param>
        /// <param name="expected"> The expected response message.</param>
        /// <returns></returns>
        [Test, TestCaseSource(typeof(UsernameValidationTestData), nameof(UsernameValidationTestData.UsernameValidationMessageCases))]
        public async Task ValidateUsernameMessage(string endpoint, string input, string expected)
        {
            // Arrange

            // Act
            var response = await _client.GetAsync($"/validation/{endpoint}?username={input}");

            // Assert
            var contentString = await response.Content.ReadAsStringAsync();

            JsonNode? message = null;
            if (!string.IsNullOrWhiteSpace(contentString))
            {
                message = JsonNode.Parse(contentString);
            }

            Console.WriteLine("Message: " + message);
            Assert.That(message, Is.Not.Null);
            Assert.That(message?["message"]?.ToString(), Is.EqualTo(expected));
        }


        [TestCase("donald-esposito121", "Username is already in use", HttpStatusCode.BadRequest)]
        [TestCase("does-not-exist5", "Accepted", HttpStatusCode.OK)]
        public async Task ValidateUsernameExists(string input, string expectedMessage, HttpStatusCode expectedStatusCode)
        {
            // Arrange

            // Act
            var response = await _client.GetAsync($"/validation/username?username={input}");
            Console.WriteLine("r,", response);

            // Assert
            var contentString = await response.Content.ReadAsStringAsync();

            JsonNode? message = null;
            if (!string.IsNullOrWhiteSpace(contentString))
            {
                message = JsonNode.Parse(contentString);
            }

            Console.WriteLine("Message: " + message);
            Assert.That(message, Is.Not.Null);
            Assert.That(message?["message"]?.ToString(), Is.EqualTo(expectedMessage));
            Assert.That(response.StatusCode, Is.EqualTo(expectedStatusCode));
        }

        [TestCase("donald-esposito121", "GitHub username is already in use", HttpStatusCode.BadRequest)]
        [TestCase("does-not-exist5", "Accepted", HttpStatusCode.OK)]
        public async Task ValidateGitUsernameExists(string input, string expectedMessage, HttpStatusCode expectedStatusCode)
        {
            // Arrange

            // Act
            var response = await _client.GetAsync($"/validation/git-username?username={input}");

            // Assert
            var contentString = await response.Content.ReadAsStringAsync();

            JsonNode? message = null;
            if (!string.IsNullOrWhiteSpace(contentString))
            {
                message = JsonNode.Parse(contentString);
            }

            Console.WriteLine("Message: " + message);
            Assert.That(message, Is.Not.Null);
            Assert.That(message?["message"]?.ToString(), Is.EqualTo(expectedMessage));
            Assert.That(response.StatusCode, Is.EqualTo(expectedStatusCode));
        }

        [TestCase("nigel.nowak2@example.com", "Email already exists", HttpStatusCode.BadRequest)]
        [TestCase("valid@email.com.no", "Accepted", HttpStatusCode.OK)]
        public async Task ValidateEmailExists(string input, string expectedMessage, HttpStatusCode expectedStatusCode)
        {
            // Arrange

            // Act
            var response = await _client.GetAsync($"/validation/email/{input}");

            // Assert
            var contentString = await response.Content.ReadAsStringAsync();

            JsonNode? message = null;
            if (!string.IsNullOrWhiteSpace(contentString))
            {
                message = JsonNode.Parse(contentString);
            }

            Console.WriteLine("Message: " + message);
            Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo(expectedMessage));
            Assert.That(response.StatusCode, Is.EqualTo(expectedStatusCode));
        }
    }
}
