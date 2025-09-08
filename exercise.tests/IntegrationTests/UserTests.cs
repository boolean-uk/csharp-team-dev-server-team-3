using exercise.wwwapi.DTOs.Login;
using exercise.wwwapi.DTOs.Register;
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
                githubUrl = $"ole-gmailpersotn{uniqueId}",
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
                githubUrl = $"{username}{uniqueId}",
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

        [TestCase("oyvind-perez1@example.com", "short1!")] // Password too short
        [TestCase("oyvind-perez1@example.com", "alllowercase1!")] // Missing uppercase
        [TestCase("oyvind-perez1@example.com", "NoNumber!")] // Missing number
        [TestCase("oyvind-perez1@example.com", "NoSpecialChar1")] // Missing special character
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
