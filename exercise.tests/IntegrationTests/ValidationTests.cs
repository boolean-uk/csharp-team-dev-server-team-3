using exercise.wwwapi.DTOs.Validation;
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
        [TestCase(null, HttpStatusCode.BadRequest)]
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
        public async Task ValidatePasswordMessage(string input, string expected)
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
            Assert.That(message?.ToString(), Is.EqualTo(expected));
        }
    }
}
