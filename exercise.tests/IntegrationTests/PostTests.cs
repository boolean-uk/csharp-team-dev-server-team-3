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

        [Test]
        public async Task AddCommentToPost_Success()
        {
            // Arrange
            var newComment = new CreatePostCommentDTO { Userid = 1, Content = "This is a test comment!" };
            var json = JsonSerializer.Serialize(newComment);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");
            int postId = 5;

            // Act
            var response = await _client.PostAsync($"/posts/{postId}/comments", requestBody);
            var contentString = await response.Content.ReadAsStringAsync();
            var message = JsonNode.Parse(contentString);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo("Success"));

            var data = message?["data"];
            Assert.That(data, Is.Not.Null);
            Assert.That(data?["id"]?.GetValue<int>(), Is.GreaterThan(0));
            Assert.That(data?["content"]?.GetValue<string>(), Is.EqualTo(newComment.Content));

            var user = data?["user"];
            Assert.That(user, Is.Not.Null);
            Assert.That(user?["id"]?.GetValue<int>(), Is.EqualTo(newComment.Userid));
        }

        [TestCase(999, 1, "Valid content", HttpStatusCode.NotFound, "Post not found.")]
        [TestCase(5, 999, "Valid content", HttpStatusCode.NotFound, "User not found.")]
        [TestCase(5, 1, "", HttpStatusCode.BadRequest, "Comment content cannot be empty.")]
        [TestCase(5, 1, "   ", HttpStatusCode.BadRequest, "Comment content cannot be empty.")]
        public async Task AddCommentToPost_Failure(int postId, int userId, string content, HttpStatusCode expectedStatus, string expectedMessage)
        {
            // Arrange
            var newComment = new CreatePostCommentDTO { Userid = userId, Content = content };
            var json = JsonSerializer.Serialize(newComment);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"/posts/{postId}/comments", requestBody);
            var contentString = await response.Content.ReadAsStringAsync();
            var message = JsonNode.Parse(contentString);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(expectedStatus));
            Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo(expectedMessage));
        }


        [Test]
        public async Task GetCommentsForPost_Success()
        {
            // Arrange
            int postId = 5;

            // Act
            var response = await _client.GetAsync($"/posts/{postId}/comments");
            var contentString = await response.Content.ReadAsStringAsync();
            var message = JsonNode.Parse(contentString);

            Console.WriteLine(message);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo("Success"));

            var data = message?["data"]?.AsArray();
            Assert.That(data, Is.Not.Null);
            Assert.That(data?.Count, Is.GreaterThan(0));

            var firstComment = data.First();
            Assert.That(firstComment?["id"]?.GetValue<int>(), Is.GreaterThan(0));
            Assert.That(firstComment?["content"]?.GetValue<string>(), Is.Not.Null.Or.Empty);

            var user = firstComment?["user"];
            Assert.That(user, Is.Not.Null);
            Assert.That(user?["id"]?.GetValue<int>(), Is.GreaterThan(0));
        }

        [TestCase("Updated test content", HttpStatusCode.OK)]
        [TestCase("", HttpStatusCode.BadRequest)]
        [TestCase("   ", HttpStatusCode.BadRequest)]
        public async Task EditComment_WithVariousInputs(string newContent, HttpStatusCode expectedStatus)
        {
            // Arrange: First create a comment to edit
            var createDto = new CreatePostCommentDTO { Userid = 1, Content = "Original comment to edit" };
            var createJson = JsonSerializer.Serialize(createDto);
            var createRequest = new StringContent(createJson, Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/posts/5/comments", createRequest);
            var createContentString = await createResponse.Content.ReadAsStringAsync();
            var createMessage = JsonNode.Parse(createContentString);
            int commentId = createMessage!["data"]!["id"]!.GetValue<int>();

            // Arrange: Prepare the update request
            var updateDto = new CreatePostCommentDTO { Userid = 1, Content = newContent };
            var updateJson = JsonSerializer.Serialize(updateDto);
            var updateRequest = new StringContent(updateJson, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PatchAsync($"/comments/{commentId}", updateRequest);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(expectedStatus));

            if (expectedStatus == HttpStatusCode.OK)
            {
                var updateContentString = await response.Content.ReadAsStringAsync();
                var updateMessage = JsonNode.Parse(updateContentString);
                var data = updateMessage?["data"];
                Assert.That(data?["content"]?.GetValue<string>(), Is.EqualTo(newContent));
                Assert.That(data?["updatedAt"], Is.Not.Null);
            }
        }

        [Test]
        public async Task EditComment_NotFound()
        {
            // Arrange
            int nonExistentCommentId = 99999;
            var updateDto = new CreatePostCommentDTO { Userid = 1, Content = "This will fail" };
            var updateJson = JsonSerializer.Serialize(updateDto);
            var updateRequest = new StringContent(updateJson, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PatchAsync($"/comments/{nonExistentCommentId}", updateRequest);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task DeleteComment_SuccessAndThenNotFound()
        {
            // Arrange: Create a comment to delete
            var createDto = new CreatePostCommentDTO { Userid = 1, Content = "Comment to be deleted" };
            var createJson = JsonSerializer.Serialize(createDto);
            var createRequest = new StringContent(createJson, Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/posts/5/comments", createRequest);
            var createContentString = await createResponse.Content.ReadAsStringAsync();
            var createMessage = JsonNode.Parse(createContentString);
            int commentId = createMessage!["data"]!["id"]!.GetValue<int>();

            // Act: Delete the comment
            var deleteResponse = await _client.DeleteAsync($"/comments/{commentId}");

            // Assert: Deletion was successful
            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            // Act: Try to delete it again
            var deleteAgainResponse = await _client.DeleteAsync($"/comments/{commentId}");

            // Assert: Now it's not found
            Assert.That(deleteAgainResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }


        [Test]
        public async Task GetPostsByUser_Success()
        {
            // Arrange
            int userId = 1; // CHeck if this user still has posts if this test fails (manuially)

            // Act
            var response = await _client.GetAsync($"/posts/user/{userId}");
            var contentString = await response.Content.ReadAsStringAsync();
            var message = JsonNode.Parse(contentString);
            Console.WriteLine(message);
            // Assert status
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            
            Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo("Success"));
            var data = message?["data"]?.AsArray();
            Assert.That(data, Is.Not.Null);
            Assert.That(data!.Count, Is.GreaterThan(0), "Expected user 1 to have at least one post.");

            // Assert that ALL posts in the response belong to the correct user
            foreach (var post in data!)
            {
                Assert.That(post?["user"]?["id"]?.GetValue<int>(), Is.EqualTo(userId));
            }

            var firstPost = data.First();
            Assert.That(firstPost?["id"]?.GetValue<int>(), Is.GreaterThan(0));
            Assert.That(firstPost?["content"]?.GetValue<string>(), Is.Not.Null);
        }

        [Test]
        public async Task GetPostsByUser_NotFound()
        {
            // Arrange
            int nonExistentUserId = 99999;

            // Act
            var response = await _client.GetAsync($"/posts/user/{nonExistentUserId}");
            var contentString = await response.Content.ReadAsStringAsync();
            var message = JsonNode.Parse(contentString);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo("No posts found for this user"));
            Assert.That(message?["data"], Is.Null);
        }

        [Test]
        public async Task GetCommentsByUser_Success()
        {
            // Arrange
            int userId = 1; // Check if this user still has comments if this fails

            // Act
            var response = await _client.GetAsync($"/comments/user/{userId}");
            var contentString = await response.Content.ReadAsStringAsync();
            var message = JsonNode.Parse(contentString);
            Console.WriteLine(message);
            // Assert status
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo("Success"));
            var data = message?["data"]?.AsArray();
            Assert.That(data, Is.Not.Null);
            Assert.That(data!.Count, Is.GreaterThan(0), "Expected user 1 to have at least one comment.");

            // Assert that ALL comments in the response belong to the correct user
            foreach (var comment in data!)
            {
                Assert.That(comment?["user"]?["id"]?.GetValue<int>(), Is.EqualTo(userId));
            }

            var firstComment = data.First();
            Assert.That(firstComment?["id"]?.GetValue<int>(), Is.GreaterThan(0));
            Assert.That(firstComment?["content"]?.GetValue<string>(), Is.Not.Null.Or.Empty);
        }

        [Test]
        public async Task GetCommentsByUser_NotFound()
        {
            // Arrange
            int nonExistentUserId = 99999;

            // Act
            var response = await _client.GetAsync($"/comments/user/{nonExistentUserId}");
            var contentString = await response.Content.ReadAsStringAsync();
            var message = JsonNode.Parse(contentString);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo("No comments found for this user"));
            Assert.That(message?["data"], Is.Null);
        }

    }
}
