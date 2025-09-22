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
    public class EndpointStatusTest : BaseIntegrationTest
    {
        /// <summary>
        /// Verifies POST requests to unknown routes return a 404 with the canonical payload.
        /// </summary>
        [Test]
        public async Task PostCheckInvalidEndpoint()
        {

            var body = new
            {
                email = TeacherEmail,
                password = TeacherPassword
            };

            string token = await LoginAndGetToken(TeacherEmail, TeacherPassword);
            HttpResponseMessage response = await SendAuthenticatedPostAsync("/NOthisWouldNEVERHAPPENItskindaweird", token, body);

            var message = await response.ReadJsonAsync();

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
            string token = await LoginAndGetToken(TeacherEmail, TeacherPassword);
            // Act
            HttpResponseMessage response = await SendAuthenticatedGetAsync("/NOthisWouldNEVERHAPPENItskindaweird", token);


            var message = await response.ReadJsonAsync();


            Console.WriteLine("Message: " + message);
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo("Endpoint not found"));
        }
    }
}
