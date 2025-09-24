using exercise.wwwapi.DTOs.Login;
using exercise.wwwapi.DTOs.Register;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using exercise.tests.Helpers;
using exercise.wwwapi.DTOs;

namespace exercise.tests.IntegrationTests
{
    /// <summary>
    /// Integration tests exercising the user management endpoints end-to-end via the API surface.
    /// </summary>
    [TestFixture]
    public class UserTests : BaseIntegrationTest
    {
        /// <summary>
        /// Confirms that valid registration payloads yield an HTTP 201 Created response.
        /// </summary>
        /// <remarks>Test cases come from <see cref="UserTestCases.ValidRegisterCases"/> and are uniquified at runtime to avoid clashes.</remarks>
        /// <param name="username">The base username supplied by the data source.</param>
        /// <param name="email">The base email supplied by the data source.</param>
        /// <param name="password">The password candidate to register with.</param>
        [Test, TestCaseSource(typeof(UserTestCases), nameof(UserTestCases.ValidRegisterCases))]
        public async Task Register_success(string username, string email, string password)
        {
            var uniqueId = DateTime.UtcNow.ToString("yyMMddHHmmssffff");

            string uniqueUsername = username.Length > 0 ? username + uniqueId : "";

            RegisterRequestDTO body = new RegisterRequestDTO
            {
                email = $"{uniqueId}{email}",
                password = password
            };
            var json = JsonSerializer.Serialize(body);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("/users", requestBody);

            var contentString = await response.Content.ReadAsStringAsync();

            JsonNode? message = null;
            if (!string.IsNullOrWhiteSpace(contentString))
            {
                message = JsonNode.Parse(contentString);
            }

            Console.WriteLine("Message: " + message);
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        /// <summary>
        /// Verifies the registration endpoint rejects malformed payloads with non-Created responses.
        /// </summary>
        /// <remarks>Inputs are provided by <see cref="UserTestCases.InvalidRegisterCases"/>.</remarks>
        /// <param name="username">The attempted username for registration.</param>
        /// <param name="email">The attempted email for registration.</param>
        /// <param name="password">The password candidate under test.</param>
        [Test, TestCaseSource(typeof(UserTestCases), nameof(UserTestCases.InvalidRegisterCases))]
        public async Task Register_Failure(string username, string email, string password)
        {
            var uniqueId = DateTime.UtcNow.ToString("yyMMddHHmmssffff");

            string uniqueUsername = username.Length > 0 ? username + uniqueId : "";
            RegisterRequestDTO body = new RegisterRequestDTO
            {
                email = $"{uniqueId}{email}",
                password = password
            };

            var json = JsonSerializer.Serialize(body);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("/users", requestBody);

            var contentString = await response.Content.ReadAsStringAsync();

            JsonNode? message = null;
            if (!string.IsNullOrWhiteSpace(contentString))
            {
                message = JsonNode.Parse(contentString);
            }

            Console.WriteLine("Message: " + message);
            // Assert
            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.Created));
        }


        /// <summary>
        /// Ensures valid credentials receive an HTTP 200 response and a token payload.
        /// </summary>
        /// <remarks>Credentials are sourced from <see cref="UserTestCases.ValidLoginCases"/>.</remarks>
        /// <param name="email">The email address to authenticate with.</param>
        /// <param name="password">The password paired with the supplied email.</param>
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
            var response = await Client.PostAsync("/login", requestBody);

            var contentString = await response.Content.ReadAsStringAsync();

            JsonNode? message = null;
            if (!string.IsNullOrWhiteSpace(contentString))
            {
                message = JsonNode.Parse(contentString);
            }

            Console.WriteLine("Message: " + message);
            using (Assert.EnterMultipleScope())
            {
                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message, Is.Not.Null);
            }
            using (Assert.EnterMultipleScope())
            {
                Assert.That(message?["data"], Is.Not.Null);
                Assert.That(message["data"]["token"], Is.Not.Null);
            }
        }

        /// <summary>
        /// Asserts that invalid login attempts fail with the expected HTTP status and message.
        /// </summary>
        /// <remarks>Negative credential combinations come from <see cref="UserTestCases.InvalidLoginCases"/>.</remarks>
        /// <param name="email">The incorrect email supplied to the login endpoint.</param>
        /// <param name="password">The incorrect password paired with the email.</param>
        [Test, TestCaseSource(typeof(UserTestCases), nameof(UserTestCases.InvalidLoginCases))]
        public async Task Login_failure(string email, string password)
        {
            var uniqueId = DateTime.UtcNow.ToString("yyMMddHHmmssffff");
            LoginRequestDTO body = new LoginRequestDTO
            {
                email = $"{uniqueId}{email}",
                password = password
            };

            var json = JsonSerializer.Serialize(body);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("/login", requestBody);

            var contentString = await response.Content.ReadAsStringAsync();

            JsonNode? message = null;
            if (!string.IsNullOrWhiteSpace(contentString))
            {
                message = JsonNode.Parse(contentString);
            }

            Console.WriteLine("Message: " + message);
            using (Assert.EnterMultipleScope())
            {
                // Assert
                Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.OK));
                Assert.That(message, Is.Not.Null);
            }

            Assert.That(message["message"], Is.Not.Null);
            Assert.That(message["message"]!.GetValue<string>(), Is.EqualTo("Invalid email and/or password provided"));

        }

        /// <summary>
        /// Validates that a PATCH with complete, valid fields updates an existing user successfully.
        /// </summary>
        [Test]
        public async Task UpdateUserSuccess()
        {
            var fieldsToUpdate = new UserPatchDTO()
            {
                Username = "roman-olsen13",
                Email = "roman.olsen13@example.com",
                Password = "aGoodPassword!200",
                Role = 0
            };

            string token = await LoginAndGetToken(TeacherEmail, TeacherPassword);
            int userId = 13;
            var response = await SendAuthenticatedPatchAsync($"/users/{userId}", token, fieldsToUpdate);


            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        /// <summary>
        /// Ensures PATCH requests with no mutable fields return a bad request response.
        /// </summary>
        [Test]
        public async Task UpdateUserNullFieldsOnly()
        {
            var fieldsToUpdate = new UserPatchDTO();

            string token = await LoginAndGetToken(TeacherEmail, TeacherPassword);
            int userId = 1;

            var response = await SendAuthenticatedPatchAsync($"/users/{userId}", token, fieldsToUpdate);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        /// <summary>
        /// Confirms the API rejects usernames that violate allowed formatting rules.
        /// </summary>
        [Test]
        public async Task UpdateUserInvalidUsername()
        {
            var fieldsToUpdate = new UserPatchDTO()
            {
                Username = "roman--olsen13"
            };

            int userId = 13;
            string token = await LoginAndGetToken(TeacherEmail, TeacherPassword);

            var response = await SendAuthenticatedPatchAsync($"/users/{userId}", token, fieldsToUpdate);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        /// <summary>
        /// Confirms GitHub username updates honor the same validation rules as the standalone validator.
        /// </summary>
        [Test]
        public async Task UpdateUserInvalidGitHubUsername()
        {
            var fieldsToUpdate = new UserPatchDTO()
            {
                GithubUsername = "roman--olsen13"
            };

            int userId = 13;
            string token = await LoginAndGetToken(TeacherEmail, TeacherPassword);

            var response = await SendAuthenticatedPatchAsync($"/users/{userId}", token, fieldsToUpdate);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        /// <summary>
        /// Verifies email updates are blocked when the address fails format validation.
        /// </summary>
        [Test]
        public async Task UpdateUserInvalidEmail()
        {
            var fieldsToUpdate = new UserPatchDTO()
            {
                Email = "roman.olsen13@.e.com"
            };

            var token = await LoginAndGetToken(TeacherEmail, TeacherPassword);

            int userId = 13;
            var response = await SendAuthenticatedPatchAsync($"/users/{userId}", token, fieldsToUpdate);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        /// <summary>
        /// Confirms passwords that do not meet the complexity requirements are rejected on update.
        /// </summary>
        [Test]
        public async Task UpdateUserInvalidPassword()
        {
            var fieldsToUpdate = new Dictionary<string, object?>
            {
                { "password", "nope!"}
            };

            var token = await LoginAndGetToken(TeacherEmail, TeacherPassword);

            int userId = 13;
            var response = await SendAuthenticatedPatchAsync($"/users/{userId}", token, fieldsToUpdate);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        /// <summary>
        /// Ensures role updates outside the defined enum range produce a bad request.
        /// </summary>
        [Test]
        public async Task UpdateUserInvalidRole()
        {
            var fieldsToUpdate = new Dictionary<string, object?>
            {
                { "role", 10}
            };

            var token = await LoginAndGetToken(TeacherEmail, TeacherPassword);

            int userId = 13;
            var response = await SendAuthenticatedPatchAsync($"/users/{userId}", token, fieldsToUpdate);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        /// <summary>
        /// Verifies the API prevents changing a username to one that is already in use.
        /// </summary>
        [Test]
        public async Task UpdateUserUsernameExists()
        {
            var fieldsToUpdate = new Dictionary<string, object?>
            {
                { "username", "nigel-nowak2"}
            };

            var token = await LoginAndGetToken(TeacherEmail, TeacherPassword);

            int userId = 13;
            var response = await SendAuthenticatedPatchAsync($"/users/{userId}", token, fieldsToUpdate);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        /// <summary>
        /// Verifies the API prevents reusing an existing GitHub username value.
        /// </summary>
        [Test]
        public async Task UpdateUserGitHubUsernameExists()
        {
            var fieldsToUpdate = new UserPatchDTO()
            {
                GithubUsername = "nigel-nowak2"
            };

            int userId = 13;
            string token = await LoginAndGetToken(TeacherEmail, TeacherPassword);

            var response = await SendAuthenticatedPatchAsync($"/users/{userId}", token, fieldsToUpdate);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        /// <summary>
        /// Verifies the API prevents reusing an existing email address value.
        /// </summary>
        [Test]
        public async Task UpdateUserEmailExists()
        {
            var fieldsToUpdate = new UserPatchDTO()
            {
                Email = "nigel.nowak2@example.com"
            };

            int userId = 13;
            string token = await LoginAndGetToken(TeacherEmail, TeacherPassword);

            var response = await SendAuthenticatedPatchAsync($"/users/{userId}", token, fieldsToUpdate);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        /// <summary>
        /// Ensures retrieving users by id returns 200 for known users and 404 for missing ones.
        /// </summary>
        /// <param name="id">The user id sent to the endpoint.</param>
        /// <param name="responseStatus">The expected HTTP status code.</param>
        [TestCase("1", HttpStatusCode.OK)]
        [TestCase("10000000", HttpStatusCode.NotFound)]
        public async Task GetUserByIdTest(string id, HttpStatusCode responseStatus)
        {
            string token = await LoginAndGetToken(TeacherEmail, TeacherPassword);

            var response = await SendAuthenticatedGetAsync($"/users/{id}", token);
            Assert.That(response.StatusCode, Is.EqualTo(responseStatus));

        }
    }
}
