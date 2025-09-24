using exercise.wwwapi.DTOs.Login;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace exercise.tests.IntegrationTests
{
    public static class HttpResponseExtensions
    {
        public static async Task<JsonNode?> ReadJsonAsync(this HttpResponseMessage response)
        {
            var contentString = await response.Content.ReadAsStringAsync();
            return string.IsNullOrWhiteSpace(contentString)
                ? null
                : JsonNode.Parse(contentString);
        }
    }
    public abstract class BaseIntegrationTest
    {
        protected WebApplicationFactory<Program> _factory;
        protected HttpClient _client;

        [SetUp]
        public void BaseSetup()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [TearDown]
        public void BaseTearDown()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }


        // IF CHANGES DONE TO THE SEEDER, RECHECK THESE USERS
        // SQL QUERY TO CHECK IF USER HAS ATLEAST ONE POST AND ATLEAST ONE COMMENT
        /*
         SELECT
            u.id AS user_id,
            u.email,
            u.role,
            ARRAY_AGG(DISTINCT p.id) AS post_ids,
            ARRAY_AGG(DISTINCT c.id) AS comment_ids
        FROM users u
        JOIN posts p ON u.id = p.userid
        JOIN comments c ON u.id = c.userid
        WHERE u.role = 'student'
        GROUP BY u.id, u.email, u.role, u.passwordhash;
         */
        protected const string TeacherEmail = "anna.gruber160@example.com"; // has post id 34,35 and comment id 37
        protected const string TeacherPassword = "Neidintulling!l33t";
        protected const int TeacherId = 160;
        protected const int TeacherPostID = 34;
        protected const int TeacherCommentID = 37;

        protected const string StudentEmail1 = "jan.larsen9@example.com"; //id 232, has post id 57, 58 and comment id 2
        protected const string StudentPassword1 = "SuperHash!4";
        protected const int StudentId1 = 9;
        protected const int StudentPostID1 = 57;
        protected const int StudentCommentID1 = 2;

        protected const string StudentEmail2 = "timian.saar85@example.com"; //id 85, has post id 36 and comment id 3
        protected const string StudentPassword2 = "Neidintulling!l33t";
        protected const int StudentPostID2 = 36;
        protected const int StudentCommentID2 = 3;


        protected async Task<string> LoginAndGetToken(string email, string password, bool success = true)
        {
            var loginBody = new LoginRequestDTO { email = email, password = password };
            var loginRequestBody = new StringContent(
                JsonSerializer.Serialize(loginBody),
                Encoding.UTF8,
                "application/json"
            );

            var loginResponse = await _client.PostAsync("/login", loginRequestBody);
            //loginResponse.EnsureSuccessStatusCode();

            var loginContent = await loginResponse.Content.ReadAsStringAsync();
            var loginMessage = JsonNode.Parse(loginContent);
            string? token = loginMessage?["data"]?["token"]?.GetValue<string>();

            Assert.That(token, success ? Is.Not.Null : Is.Null);

            return token;
        }

        // Helper methods
        protected async Task<HttpResponseMessage> SendAuthenticatedRequestAsync<T>(
            HttpMethod method,
            string endpoint,
            string token,
            T? body = default
            )
        {
            var request = new HttpRequestMessage(method, endpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (body != null)
            {
                var json = JsonSerializer.Serialize(body);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            return await _client.SendAsync(request);
        }

        protected async Task<HttpResponseMessage> SendAuthenticatedGetAsync(string endpoint, string token)
        {
            return await SendAuthenticatedRequestAsync<object>(HttpMethod.Get, endpoint, token);
        }

        protected async Task<HttpResponseMessage> SendAuthenticatedPostAsync<T>(string endpoint, string token, T body)
        {
            return await SendAuthenticatedRequestAsync(HttpMethod.Post, endpoint, token, body);
        }
        protected async Task<HttpResponseMessage> SendAuthenticatedPatchAsync<T>(string endpoint, string token, T body)
        {
            return await SendAuthenticatedRequestAsync(HttpMethod.Patch, endpoint, token, body);
        }

        protected async Task<HttpResponseMessage> SendAuthenticatedPutAsync<T>(string endpoint, string token, T body)
        {
            return await SendAuthenticatedRequestAsync(HttpMethod.Put, endpoint, token, body);
        }

        protected async Task<HttpResponseMessage> SendAuthenticatedPostAsync(string endpoint, string token)
        {
            return await SendAuthenticatedRequestAsync<object>(HttpMethod.Post, endpoint, token);
        }

        protected async Task<HttpResponseMessage> SendAuthenticatedDeleteAsync(string endpoint, string token)
        {
            return await SendAuthenticatedRequestAsync<object>(HttpMethod.Delete, endpoint, token);
        }

    }
}
