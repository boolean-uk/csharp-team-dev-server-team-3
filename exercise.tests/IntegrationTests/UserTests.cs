using exercise.wwwapi.DTOs;
using exercise.wwwapi.DTOs.Login;
using exercise.wwwapi.DTOs.Register;
using exercise.wwwapi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

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
        [Test] 
        public async Task Register_success()
        {
            var uniqueId = DateTime.UtcNow.ToString("yyMMddHHmmssffff");
            RegisterRequestDTO body = new RegisterRequestDTO { 
                email= $"myemailVery{uniqueId}@gmail.com",
                firstName = "Ole",
                lastName = "Petterson",
                bio = "Min bio er vakker",
                githubUsername = $"ole-gmailpersotn{uniqueId}",
                username= $"ole-perrston{uniqueId}",
                password = "someR21!password"
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

        // Uncomment these when email/username validation has been implemented in the endpoint
        //[TestCase("validuser", "plainaddress", "ValidPass1!")] // Invalid email format
        //[TestCase("validuser", "user@domain.c", "ValidPass1!")] // Invalid email domain
        //[TestCase("ThisIsWayTooLong123", "valid@email.com", "ValidPass1!")] // Username too long
        //[TestCase("", "valid@email.com", "ValidPass1!")] // Username too short, change logic so it doesnt add uniqueid to username
        [TestCase("validuser", "valid@email.com", "short1!")] // Password too short
        [TestCase("validuser", "valid@email.com", "alllowercase1!")] // Missing uppercase
        [TestCase("validuser", "valid@email.com", "NoNumber!")] // Missing number
        [TestCase("validuser", "valid@email.com", "NoSpecialChar1")] // Missing special character
        public async Task Register_Failure(string username, string email, string password)
        {
            var uniqueId = DateTime.UtcNow.ToString("yyMMddHHmmssffff");
            string firstName = "Ole";
            string lastName = "Petterson";
            RegisterRequestDTO body = new RegisterRequestDTO
            {
                email = $"myemailVery{uniqueId}@gmail.com",
                firstName = firstName,
                lastName = lastName,
                bio = "Min bio er vakker",
                githubUsername = $"{username}{uniqueId}",
                username = $"{username}{uniqueId}",
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


        [Test]
        public async Task Login_success()
        {
            var uniqueId = DateTime.UtcNow.ToString("yyMMddHHmmssffff");
            RegisterRequestDTO body = new RegisterRequestDTO
            {
                email = "oyvind.perez1@example.com",
                password = "SuperHash!4"
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

        [TestCase("oyvind.perez1@example.com", "short1!")] // Password too short
        [TestCase("oyvind.perez1@example.com", "alllowercase1!")] // Missing uppercase
        [TestCase("oyvind.perez1@example.com", "NoNumber!")] // Missing number
        [TestCase("oyvind.perez1@example.com", "NoSpecialChar1")] // Missing special character
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

        [Test]
        public async Task UpdateUserSuccess()
        {
            var fieldsToUpdate = new Dictionary<string, object?>
            {
                { "username", "roman-olsen13"},
                { "email", "roman.olsen13@example.com"},
                { "password", "aGoodPassword!200" },
                { "role", 0}
            };

            var json = JsonSerializer.Serialize(fieldsToUpdate);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            int userId = 13;
            var response = await _client.PatchAsync($"/users/{userId}", content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task UpdateUserNoContent()
        {
            var fieldsToUpdate = new Dictionary<string, object?>{};

            var json = JsonSerializer.Serialize(fieldsToUpdate);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            int userId = 1;
            var response = await _client.PatchAsync($"/users/{userId}", content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public async Task UpdateUserInvalidUsername()
        {
            var fieldsToUpdate = new Dictionary<string, object?>
            {
                { "username", "roman--olsen13"}
            };

            var json = JsonSerializer.Serialize(fieldsToUpdate);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            int userId = 13;
            var response = await _client.PatchAsync($"/users/{userId}", content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task UpdateUserInvalidGitHubUsername()
        {
            var fieldsToUpdate = new Dictionary<string, object?>
            {
                { "gitHubUsername", "roman--olsen13"}
            };

            var json = JsonSerializer.Serialize(fieldsToUpdate);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            int userId = 13;
            var response = await _client.PatchAsync($"/users/{userId}", content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task UpdateUserInvalidEmail()
        {
            var fieldsToUpdate = new Dictionary<string, object?>
            {
                { "email", "roman.olsen13@.e.com"}
            };

            var json = JsonSerializer.Serialize(fieldsToUpdate);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            int userId = 13;
            var response = await _client.PatchAsync($"/users/{userId}", content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task UpdateUserInvalidPassword()
        {
            var fieldsToUpdate = new Dictionary<string, object?>
            {
                { "password", "nope!"}
            };

            var json = JsonSerializer.Serialize(fieldsToUpdate);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            int userId = 13;
            var response = await _client.PatchAsync($"/users/{userId}", content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task UpdateUserInvalidRole()
        {
            var fieldsToUpdate = new Dictionary<string, object?>
            {
                { "role", 10}
            };

            var json = JsonSerializer.Serialize(fieldsToUpdate);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            int userId = 13;
            var response = await _client.PatchAsync($"/users/{userId}", content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task UpdateUserUsernameExists()
        {
            var fieldsToUpdate = new Dictionary<string, object?>
            {
                { "username", "nigel-nowak2"}
            };

            var json = JsonSerializer.Serialize(fieldsToUpdate);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            int userId = 13;
            var response = await _client.PatchAsync($"/users/{userId}", content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task UpdateUserGitHubUsernameExists()
        {
            var fieldsToUpdate = new Dictionary<string, object?>
            {
                { "gitHubUsername", "nigel-nowak2"}
            };

            var json = JsonSerializer.Serialize(fieldsToUpdate);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            int userId = 13;
            var response = await _client.PatchAsync($"/users/{userId}", content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task UpdateUserEmailExists()
        {
            var fieldsToUpdate = new Dictionary<string, object?>
            {
                { "email", "nigel.nowak2@example.com"}
            };

            var json = JsonSerializer.Serialize(fieldsToUpdate);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            int userId = 13;
            var response = await _client.PatchAsync($"/users/{userId}", content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

    }
}
