using exercise.wwwapi.DTOs.Posts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace exercise.tests.IntegrationTests
{
    [TestFixture]
    public class PostTests
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

        [Test]
        public async Task GetAllPosts()
        {
            // Act: get all posts
            var response = await _client.GetAsync("/posts");
            var contentString = await response.Content.ReadAsStringAsync();
            var message = string.IsNullOrWhiteSpace(contentString) ? null : JsonNode.Parse(contentString);

            // Assert status
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            // Assert JSON structure
            Assert.That(message, Is.Not.Null);
            Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo("success"));

            var data = message?["data"]?.AsArray();
            Assert.That(data, Is.Not.Null);
            Assert.That(data!.Count, Is.GreaterThan(0));


            Console.WriteLine("Message: " + message);

            // Check first post for structure
            var post = data!.First();
            Assert.That(post["id"]?.GetValue<int>(), Is.GreaterThan(0));
            Assert.That(post["content"]?.GetValue<string>(), Is.Not.Null);
            Assert.That(post["numLikes"]?.GetValue<int>(), Is.Not.Null);
            Assert.That(post["createdAt"], Is.Not.Null);

            // Check nested user
            var user = post["user"];
            Assert.That(user, Is.Not.Null);
            Assert.That(user!["id"]?.GetValue<int>(), Is.GreaterThan(0));
            Assert.That(user["firstName"]?.GetValue<string>(), Is.Not.Null);
            Assert.That(user["lastName"]?.GetValue<string>(), Is.Not.Null);
            Assert.That(user["photo"], Is.Not.Null);

            // Check nested comments
            var comments = post["comments"]?.AsArray();
            Assert.That(comments, Is.Not.Null);
            // Check comments
            if (comments!.Count > 0)
            {
                var comment = comments!.First();
                Assert.That(comment["content"]?.GetValue<string>(), Is.Not.Null);
                Assert.That(comment["id"]?.GetValue<int>(), Is.GreaterThan(0));
            }
        }

        [Test]
        public async Task SuccessfulCreatePostStatus()
        {
            int userid = 1;
            string content = "Some content";
            CreatePostDTO body = new CreatePostDTO
            {
                Userid = userid,
                Content = content
            };
            var json = JsonSerializer.Serialize(body);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/posts", requestBody);

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

        [Test]
        public async Task SuccessfulCreatePostMessage()
        {
            int userid = 1;
            string content = "Some content";
            CreatePostDTO body = new CreatePostDTO
            {
                Userid = userid,
                Content = content
            };
            var json = JsonSerializer.Serialize(body);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/posts", requestBody);

            var contentString = await response.Content.ReadAsStringAsync();

            JsonNode? message = null;
            if (!string.IsNullOrWhiteSpace(contentString))
            {
                message = JsonNode.Parse(contentString);
            }

            Console.WriteLine("Message: " + message);
            // Assert

            // Assert JSON structure
            Assert.That(message, Is.Not.Null);
            Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo("success"));

            var data = message["data"];
            Assert.That(data, Is.Not.Null);
            Assert.That(data?["id"]?.GetValue<int>(), Is.GreaterThan(0));
            Assert.That(data["content"]?.GetValue<string>(), Is.EqualTo(content));
            Assert.That(data["numLikes"]?.GetValue<int>(), Is.EqualTo(0));
            Assert.That(data["comments"]?.AsArray().Count, Is.EqualTo(0));

            // Check User object inside data
            var user = data["user"];
            Assert.That(user, Is.Not.Null);
            Assert.That(user!["id"]?.GetValue<int>(), Is.EqualTo(userid));
            Assert.That(user["firstName"]?.GetValue<string>(), Is.Not.Null);
            Assert.That(user["lastName"]?.GetValue<string>(), Is.Not.Null);
            Assert.That(user["photo"]?.GetValue<string>(), Is.Not.Null);

            Assert.That(message["timestamp"], Is.Not.Null);
            Assert.That(data["createdAt"], Is.Not.Null);
        }

        [TestCase(9999999, "somecontent", HttpStatusCode.NotFound)]
        [TestCase(5, "", HttpStatusCode.BadRequest)]
        [TestCase(5, "      ", HttpStatusCode.BadRequest)]
        [TestCase(5, "          ", HttpStatusCode.BadRequest)]
        public async Task FailedlCreatePostStatus(int userid, string content, HttpStatusCode expected)
        {
            CreatePostDTO body = new CreatePostDTO
            {
                Userid = userid,
                Content = content
            };
            var json = JsonSerializer.Serialize(body);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/posts", requestBody);

            var contentString = await response.Content.ReadAsStringAsync();

            JsonNode? message = null;
            if (!string.IsNullOrWhiteSpace(contentString))
            {
                message = JsonNode.Parse(contentString);
            }

            Console.WriteLine("Message: " + message);
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(expected));
        }

        [TestCase(9999999, "somecontent", "Invalid userID")]
        [TestCase(5, "", "Content cannot be empty")]
        [TestCase(5, "      ", "Content cannot be empty")]
        [TestCase(5, "          ", "Content cannot be empty")]
        public async Task FailedCreatePostMessage(int userid, string content, string expectedmessage)
        {
            CreatePostDTO body = new CreatePostDTO
            {
                Userid = userid,
                Content = content
            };
            var json = JsonSerializer.Serialize(body);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/posts", requestBody);

            var contentString = await response.Content.ReadAsStringAsync();

            JsonNode? message = null;
            if (!string.IsNullOrWhiteSpace(contentString))
            {
                message = JsonNode.Parse(contentString);
            }

            Console.WriteLine("Message: " + message);

            // Assert JSON structure
            Assert.That(message, Is.Not.Null);
            Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo(expectedmessage));
            Assert.That(message["data"], Is.Null);
            Assert.That(message["timestamp"], Is.Not.Null);
        }
        [Test]
        public async Task DeletePostById_SuccessAndNotFound()
        {
            // Arrange, create a post first
            var newPost = new CreatePostDTO
            {
                Userid = 1,
                Content = "Temp post to delete"
            };
            var json = JsonSerializer.Serialize(newPost);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");

            var createResponse = await _client.PostAsync("/posts", requestBody);
            var createContent = await createResponse.Content.ReadAsStringAsync();
            var createMessage = JsonNode.Parse(createContent);

            int createdPostId = createMessage!["data"]!["id"]!.GetValue<int>();

            // Act, delete the post
            var deleteResponse = await _client.DeleteAsync($"/posts/{createdPostId}");
            var deleteContent = await deleteResponse.Content.ReadAsStringAsync();

            // Assert delete success
            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            // Act again, try deleting same post (should now be gone)
            var deleteAgainResponse = await _client.DeleteAsync($"/posts/{createdPostId}");
            var deleteAgainContent = await deleteAgainResponse.Content.ReadAsStringAsync();
            var deleteAgainMessage = JsonNode.Parse(deleteAgainContent);

            // Assert not found
            Assert.That(deleteAgainResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(deleteAgainMessage?["message"]?.GetValue<string>(), Is.EqualTo("Post not found"));
        }




        [TestCase(5, "Updated content", HttpStatusCode.OK)]
        [TestCase(9999999, "Updated content", HttpStatusCode.NotFound)]
        [TestCase(5, "", HttpStatusCode.BadRequest)]
        public async Task UpdatePostById(int postId, string newContent, HttpStatusCode expected)
        {
            // Arrange
            UpdatePostDTO body = new UpdatePostDTO { Content = newContent };
            var json = JsonSerializer.Serialize(body);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PatchAsync($"/posts/{postId}", requestBody);
            var contentString = await response.Content.ReadAsStringAsync();

            JsonNode? message = null;
            if (!string.IsNullOrWhiteSpace(contentString))
            {
                message = JsonNode.Parse(contentString);
            }

            Console.WriteLine("Message: " + message);

            // Assert status
            Assert.That(response.StatusCode, Is.EqualTo(expected));

            if (expected == HttpStatusCode.OK)
            {
                var data = message?["data"];
                Assert.That(data, Is.Not.Null);
                Assert.That(data?["id"]?.GetValue<int>(), Is.EqualTo(postId));
                Assert.That(data?["content"]?.GetValue<string>(), Is.EqualTo(newContent));
                Assert.That(data?["updatedAt"], Is.Not.Null);
            }
            else if (expected == HttpStatusCode.NotFound)
            {
                Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo("Post not found"));
                Assert.That(message?["data"], Is.Null);
            }
            else if (expected == HttpStatusCode.BadRequest)
            {
                Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo("Content cannot be empty"));
                Assert.That(message?["data"], Is.Null);
            }
        }


    }
}
