using exercise.wwwapi.DTOs.Login;
using exercise.wwwapi.DTOs.Register;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using exercise.tests.Helpers;

namespace exercise.tests.IntegrationTests
{
    [TestFixture]
    public class UserTests
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

        // ad test cases for approved usernames, emails
        [Test, TestCaseSource(typeof(UserTestCases), nameof(UserTestCases.ValidRegisterCases))] 
        public async Task Register_success(string username, string email, string password)
        {
            var uniqueId = DateTime.UtcNow.ToString("yyMMddHHmmssffff");

            string k = username.Length > 0 ? username + uniqueId : "";

            RegisterRequestDTO body = new RegisterRequestDTO {
                email = $"{uniqueId}{email}",
                firstName = "Ole",
                lastName = "Petterson",
                bio = "Min bio er vakker",
                githubUsername = username,
                username = username,
                password = password
            };
            var json = JsonSerializer.Serialize(body);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/users", requestBody);

            var contentString = await response.Content.ReadAsStringAsync();

            JsonNode? message = null;
            if (!string.IsNullOrWhiteSpace(contentString))
            {
                message = JsonNode.Parse(contentString);
            }

            Console.WriteLine("Message: " + message);
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test, TestCaseSource(typeof(UserTestCases), nameof(UserTestCases.InvalidRegisterCases))]
        public async Task Register_Failure(string username, string email, string password)
        {
            var uniqueId = DateTime.UtcNow.ToString("yyMMddHHmmssffff");
            string firstName = "Ole";
            string lastName = "Petterson";

            string k = username.Length > 0 ? username +uniqueId : "";
            RegisterRequestDTO body = new RegisterRequestDTO
            {
                email = $"{uniqueId}{email}",
                firstName = firstName,
                lastName = lastName,
                bio = "Min bio er vakker",
                githubUsername = username,
                username =username,
                password = password
            };
            
            var json = JsonSerializer.Serialize(body);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/users", requestBody);

            var contentString = await response.Content.ReadAsStringAsync();

            JsonNode? message = null;
            if (!string.IsNullOrWhiteSpace(contentString))
            {
                message = JsonNode.Parse(contentString);
            }

            Console.WriteLine("Message: " + message);
            // Assert
            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.OK));
        }


        [Test, TestCaseSource(typeof(UserTestCases), nameof(UserTestCases.ValidLoginCases))]
        public async Task Login_success(string email, string password)
        {
            var uniqueId = DateTime.UtcNow.ToString("yyMMddHHmmssffff");
            RegisterRequestDTO body = new RegisterRequestDTO
            {
                email = email,
                password = password
            };
            var json = JsonSerializer.Serialize(body);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/login", requestBody);

            var contentString = await response.Content.ReadAsStringAsync();

            JsonNode? message = null;
            if (!string.IsNullOrWhiteSpace(contentString))
            {
                message = JsonNode.Parse(contentString);
            }

            Console.WriteLine("Message: " + message);
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(message, Is.Not.Null);
            Assert.That(message?["data"], Is.Not.Null);
            Assert.That(message["data"]["token"], Is.Not.Null);
        }

        [Test, TestCaseSource(typeof(UserTestCases), nameof(UserTestCases.InvalidLoginCases))]
        public async Task Login_failure(string email, string password)
        {
            var uniqueId = DateTime.UtcNow.ToString("yyMMddHHmmssffff");
            LoginRequestDTO body = new LoginRequestDTO
            {
                email = email,
                password = password
            };

            var json = JsonSerializer.Serialize(body);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/login", requestBody);

            var contentString = await response.Content.ReadAsStringAsync();

            JsonNode? message = null;
            if (!string.IsNullOrWhiteSpace(contentString))
            {
                message = JsonNode.Parse(contentString);
            }

            Console.WriteLine("Message: " + message);
            // Assert
            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.OK));
            Assert.That(message, Is.Not.Null);
            Assert.That(message["data"], Is.Not.Null);
            Assert.That(message["data"]!.GetValue<string>(), Is.EqualTo("Invalid email and/or password provided"));
        }
    }
}
