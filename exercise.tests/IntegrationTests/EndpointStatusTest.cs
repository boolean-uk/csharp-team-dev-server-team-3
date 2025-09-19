using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace exercise.tests.IntegrationTests
{
    /// <summary>
    /// Integration smoke tests that ensure unexpected endpoints return consistent error responses.
    /// </summary>
    [TestFixture]
    public class EndpointStatusTest
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

        /// <summary>
        /// Verifies POST requests to unknown routes return a 404 with the canonical payload.
        /// </summary>
        [Test]
        public async Task PostCheckInvalidEndpoint()
        {
            var body = new
            {
                email = $"myemailVery@gmail.com",
                password = "someR21!password"
            };
            var json = JsonSerializer.Serialize(body);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/okokoNONEVER", requestBody);

            var contentString = await response.Content.ReadAsStringAsync();

            JsonNode? message = null;
            if (!string.IsNullOrWhiteSpace(contentString))
            {
                message = JsonNode.Parse(contentString);
            }

            var messageText = message?["message"]?.GetValue<string>();

            Console.WriteLine("Message: " + message);
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo("Endpoint not found"));
        }

        /// <summary>
        /// Verifies GET requests to unknown routes return a 404 with the canonical payload.
        /// </summary>
        [Test]
        public async Task GetCheckInvalidEndpoint()
        {

            // Act
            var response = await _client.GetAsync("/okokoNONEVER");

            var contentString = await response.Content.ReadAsStringAsync();

            JsonNode? message = null;
            if (!string.IsNullOrWhiteSpace(contentString))
            {
                message = JsonNode.Parse(contentString);
            }

            var messageText = message?["message"]?.GetValue<string>();

            Console.WriteLine("Message: " + message);
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo("Endpoint not found"));
        }
    }
}
