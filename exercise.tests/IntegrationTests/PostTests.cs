using exercise.wwwapi.DTOs.Login;
using exercise.wwwapi.DTOs.Posts;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace exercise.tests.IntegrationTests
{
    /// <summary>
    /// Integration tests covering the lifecycle of posts via the public API endpoints.
    /// </summary>
    [TestFixture]
    public class PostTests : BaseIntegrationTest
    {
        [Test]
        public async Task GetAllPosts()
        {
            //Assert get token
            string token = await LoginAndGetToken("oyvind.perez1@example.com", "SuperHash!4");
            //act
            HttpResponseMessage response = await SendAuthenticatedGetAsync($"/posts", token);
            JsonNode? message = await response.ReadJsonAsync();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message, Is.Not.Null);
                Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo("Success"));
            }
            var data = message?["data"]?.AsArray();
            Assert.That(data, Is.Not.Null);
            Assert.That(data?.Count, Is.GreaterThan(0));
            


            Console.WriteLine("Message: " + message);

            // Check first post for structure
            var post = data?.First();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(post?["id"]?.GetValue<int>(), Is.GreaterThan(0));
                Assert.That(post["content"]?.GetValue<string>(), Is.Not.Null);
                Assert.That(post["numLikes"]?.GetValue<int>(), Is.Not.Null);
                Assert.That(post["createdAt"], Is.Not.Null);
            }

            // Check nested user
            var user = post["user"];
            Assert.That(user, Is.Not.Null);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(user?["id"]?.GetValue<int>(), Is.GreaterThan(0));
                Assert.That(user["firstName"]?.GetValue<string>(), Is.Not.Null);
                Assert.That(user["lastName"]?.GetValue<string>(), Is.Not.Null);
                Assert.That(user["photo"], Is.Not.Null);
            }

            // Check nested comments
            var comments = post["comments"]?.AsArray();
            Assert.That(comments, Is.Not.Null);
            // Check comments
            if (comments?.Count > 0)
            {
                var comment = comments?.First();
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(comment?["content"]?.GetValue<string>(), Is.Not.Null);
                    Assert.That(comment?["id"]?.GetValue<int>(), Is.GreaterThan(0));
                }
            }
        }

       

        [TestCase(TeacherEmail, TeacherPassword)]
        [TestCase(StudentEmail1, StudentPassword1)]
        public async Task SuccessfulCreatePostStatus(string userEmail, string password)
        {
            string token = await LoginAndGetToken(userEmail, password);
            CreatePostDTO body = new() { Content = "Some content" };

            //Act
            HttpResponseMessage response = await SendAuthenticatedPostAsync("/posts", token, body);
            JsonNode? message = await response.ReadJsonAsync();


            Console.WriteLine("Message: " + message);
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        [TestCase(TeacherEmail, TeacherPassword)]
        [TestCase(StudentEmail1, StudentPassword1)]
        public async Task SuccessfulCreatePostMessage(string userEmail, string password)
        {
            string token = await LoginAndGetToken(userEmail, password);
            CreatePostDTO body = new(){ Content = "Some content" };

            //Act
            HttpResponseMessage response = await SendAuthenticatedPostAsync("/posts", token, body);
            JsonNode? message = await response.ReadJsonAsync();

            Console.WriteLine("Message: " + message);
            // Assert

            // Assert JSON structure
            Assert.That(message, Is.Not.Null);
            Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo("Success"));

            var data = message["data"];
            Assert.That(data, Is.Not.Null);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(data?["id"]?.GetValue<int>(), Is.GreaterThan(0));
                Assert.That(data["content"]?.GetValue<string>(), Is.EqualTo("Some content"));
                Assert.That(data["numLikes"]?.GetValue<int>(), Is.EqualTo(0));
                Assert.That(data["comments"]?.AsArray().Count, Is.EqualTo(0));
            }

            // Check User object inside data
            var user = data["user"];
            Assert.That(user, Is.Not.Null);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(user["firstName"]?.GetValue<string>(), Is.Not.Null);
                Assert.That(user["lastName"]?.GetValue<string>(), Is.Not.Null);
                Assert.That(user["photo"]?.GetValue<string>(), Is.Not.Null);
                Assert.That(message["timestamp"], Is.Not.Null);
                Assert.That(data["createdAt"], Is.Not.Null);
            }
        }

        [TestCase(TeacherEmail, TeacherPassword, "", HttpStatusCode.BadRequest)]
        [TestCase(TeacherEmail, TeacherPassword, "      ", HttpStatusCode.BadRequest)]
        [TestCase(TeacherEmail, TeacherPassword, "          ", HttpStatusCode.BadRequest)]
        public async Task FailedCreatePostStatus(string userEmail, string password, string content, HttpStatusCode expected)
        {
            string token = await LoginAndGetToken(userEmail, password);

            CreatePostDTO body = new () { Content = content };

            //Act 
            HttpResponseMessage response = await SendAuthenticatedPostAsync("/posts", token, body);
            JsonNode? message = await response.ReadJsonAsync();

            Console.WriteLine($"Message: {message}| ");
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(expected));
        }

        [TestCase(TeacherEmail, TeacherPassword, "", "Content cannot be empty")]
        [TestCase(TeacherEmail, TeacherPassword, "      ", "Content cannot be empty")]
        [TestCase(TeacherEmail, TeacherPassword, "          ", "Content cannot be empty")]
        public async Task FailedCreatePostMessage(string userEmail, string password, string content, string expectedmessage)
        {
            string token = await LoginAndGetToken(userEmail, password);

            CreatePostDTO body = new() { Content = content };

            //Act 
            HttpResponseMessage response = await SendAuthenticatedPostAsync("/posts", token, body);
            JsonNode? message = await response.ReadJsonAsync();

            Console.WriteLine($"Message: {message}| ");

            // Assert JSON structure
            Assert.That(message, Is.Not.Null);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo(expectedmessage));
                Assert.That(message["data"], Is.Null);
                Assert.That(message["timestamp"], Is.Not.Null);
            }
        }

        [TestCase(TeacherEmail, TeacherPassword)]
        [TestCase(StudentEmail1, StudentPassword1)]
        public async Task DeletePostById_SuccessAndNotFound(string userEmail, string password)
        {
            string token = await LoginAndGetToken(userEmail, password);

            // Arrange create a post first
            CreatePostDTO body = new CreatePostDTO { Content = "A brand new post!" };

            //ACt
            HttpResponseMessage createResponse = await SendAuthenticatedPostAsync($"/posts", token, body);
            JsonNode? createMessage = await createResponse.ReadJsonAsync();

            int? createdPostId = createMessage?["data"]?["id"]?.GetValue<int>();

            Assert.That(createdPostId, Is.Not.Null);

            // Act, delete the post
            HttpResponseMessage deleteResponse = await SendAuthenticatedDeleteAsync($"/posts/{createdPostId}", token);
            JsonNode? deleteContent = await deleteResponse.ReadJsonAsync();

            // Assert delete success
            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            // Act again, try deleting same post (should now be gone)
            HttpResponseMessage deleteAgainResponse = await SendAuthenticatedDeleteAsync($"/posts/{createdPostId}", token);
            JsonNode? deleteAgainMessage = await deleteAgainResponse.ReadJsonAsync();


            // Assert not found
            Assert.That(deleteAgainResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(deleteAgainMessage?["message"]?.GetValue<string>(), Is.EqualTo("Post not found"));
        }



        [TestCase(TeacherEmail, TeacherPassword, TeacherPostID, "Teacher updated own post", HttpStatusCode.OK)]          // Teacher edit their own post
        [TestCase(TeacherEmail, TeacherPassword, StudentPostID1, "Teacher updated student’s post", HttpStatusCode.OK)]    // Teacher edits someone else’s post
        [TestCase(StudentEmail1, StudentPassword1, StudentPostID1, "Student1 updated own post", HttpStatusCode.OK)]        // Student1 edits their own post
        [TestCase(StudentEmail1, StudentPassword1, StudentPostID2, "Student1 tried updating Student2’s post", HttpStatusCode.Forbidden)]        // Student1 tries to edit Student2’s post
        [TestCase(StudentEmail2, StudentPassword2, StudentPostID2, "Student2 updated own post", HttpStatusCode.OK)]        // Student2 edits their own post
        [TestCase(StudentEmail2, StudentPassword2, TeacherPostID, "Student2 tried updating Teacher’s post", HttpStatusCode.Forbidden)]   // Student2 tries to edit Teacher’s post
        [TestCase(StudentEmail1, "Wrong!1", StudentPostID1, "Invalid login attempt", HttpStatusCode.Unauthorized)]   // Unauthenticated user (wrong password)     
        [TestCase(TeacherEmail, TeacherPassword, 999999, "Does not exist", HttpStatusCode.NotFound)]// Post does not exist
        [TestCase(TeacherEmail, TeacherPassword, TeacherPostID, "", HttpStatusCode.BadRequest)]   // Invalid content (empty string)
        [TestCase(StudentEmail1, StudentPassword1, StudentPostID2, "", HttpStatusCode.BadRequest)] // Invalid content (empty string)
        public async Task UpdatePostById(string userEmail, string password, int postId, string newContent, HttpStatusCode expected)
        {

            // Arrange: Login and get token
            string token = await LoginAndGetToken(userEmail, password, password != "Wrong!1" );


            // Arrange
            UpdatePostDTO body = new UpdatePostDTO { Content = newContent };
            HttpResponseMessage response = await SendAuthenticatedPatchAsync($"/posts/{postId}", token, body);
            JsonNode? message = await response.ReadJsonAsync();


            Console.WriteLine("Message: " + message);

            // Assert status
            Assert.That(response.StatusCode, Is.EqualTo(expected));

            if (expected == HttpStatusCode.OK)
            {
                var data = message?["data"];
                Assert.That(data, Is.Not.Null);
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(data?["id"]?.GetValue<int>(), Is.EqualTo(postId));
                    Assert.That(data?["content"]?.GetValue<string>(), Is.EqualTo(newContent));
                    Assert.That(data?["updatedAt"], Is.Not.Null);
                }
            }
            else if (expected == HttpStatusCode.NotFound)
            {
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo("Post not found"));
                    Assert.That(message?["data"], Is.Null);
                }
            }
            else if (expected == HttpStatusCode.BadRequest)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo("Content cannot be empty"));
                    Assert.That(message?["data"], Is.Null);
                });
            }
            else if (expected == HttpStatusCode.Forbidden)
            {
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo("You are not authorized to edit this post."));
                    Assert.That(message?["data"], Is.Null);
                }
            }
        }

        [TestCase(TeacherEmail, TeacherPassword)]
        [TestCase(StudentEmail1, StudentPassword1)]
        public async Task AddCommentToPost_Success(string userEmail, string password)
        {
            string token = await LoginAndGetToken(userEmail, password);

            // Arrange
            CreatePostDTO body = new CreatePostDTO { Content = "Some new post" };

            // Act
            HttpResponseMessage response = await SendAuthenticatedPostAsync("/posts", token, body);
            JsonNode? message = await response.ReadJsonAsync();

            int? createdPostId = message?["data"]?["id"]?.GetValue<int>();
            Console.WriteLine($"createdPostId:{createdPostId}.");

            Assert.That(createdPostId, Is.Not.Null);

            // Arrange
            CreatePostCommentDTO commentbody = new CreatePostCommentDTO { Content = "Some new comment" };

            // Act
            HttpResponseMessage commentresponse = await SendAuthenticatedPostAsync($"/posts/{createdPostId}/comments", token, commentbody);
            JsonNode? commentmessage = await commentresponse.ReadJsonAsync();

            Console.WriteLine(commentmessage);
            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(commentresponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));
                Assert.That(commentmessage?["message"]?.GetValue<string>(), Is.EqualTo("Success"));
            });

            var data = commentmessage?["data"];
            Assert.That(data, Is.Not.Null);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(data?["id"]?.GetValue<int>(), Is.GreaterThan(0));
                Assert.That(data?["content"]?.GetValue<string>(), Is.EqualTo(commentbody.Content));
            }

            var user = data?["user"];
            Assert.That(user, Is.Not.Null);
            //Assert.That(user?["id"]?.GetValue<int>(), Is.EqualTo(userid));
        }

        [TestCase(999, "Valid content", HttpStatusCode.NotFound, "Post not found.")]
        [TestCase(5, "", HttpStatusCode.BadRequest, "Comment content cannot be empty.")]
        [TestCase(5, "   ", HttpStatusCode.BadRequest, "Comment content cannot be empty.")]
        public async Task AddCommentToPost_Failure(int postId, string content, HttpStatusCode expectedStatus, string expectedMessage)
        {
            // Arrange
            string token = await LoginAndGetToken(TeacherEmail, TeacherPassword);
            var newComment = new CreatePostCommentDTO { Content = content };

            HttpResponseMessage response = await SendAuthenticatedPostAsync($"/posts/{postId}/comments", token, newComment);

            // Act
            JsonNode? message = await response.ReadJsonAsync();

            Console.WriteLine(message);

            using (Assert.EnterMultipleScope())
            {
                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(expectedStatus));
                Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo(expectedMessage));
            }
        }


        [Test]
        public async Task GetCommentsForPost_Success()
        {

            //Assert get token
            var token = await LoginAndGetToken("oyvind.perez1@example.com", "SuperHash!4");

            // Arrange the request with token
            int postId = 5;

            // Act
            HttpResponseMessage response = await SendAuthenticatedGetAsync($"/posts/{postId}/comments", token);

            // Assert
            JsonNode? message = await response.ReadJsonAsync();

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

        [TestCase(TeacherEmail, TeacherPassword, "Updated test content", HttpStatusCode.OK)]
        [TestCase(TeacherEmail, TeacherPassword, "", HttpStatusCode.BadRequest)]
        [TestCase(TeacherEmail, TeacherPassword, "   ", HttpStatusCode.BadRequest)]
        public async Task EditComment_WithVariousInputs(string userEmail, string password,string newContent, HttpStatusCode expectedStatus)
        {
            
            string token = await LoginAndGetToken(userEmail, password);

            // Arrange: First create a post
            CreatePostCommentDTO commentDTO = new () { Content = "Original comment to edit" };
            CreatePostDTO postDTO = new() { Content = "Some content" };

            HttpResponseMessage postRes = await SendAuthenticatedPostAsync("/posts", token, postDTO);
            Assert.That(postRes.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            JsonNode? createMessage = await postRes.ReadJsonAsync();


            int? createdPostId = createMessage?["data"]?["id"]?.GetValue<int>();

            Assert.That(createdPostId, Is.Not.Null);

            //  Arrange: Create comment
            HttpResponseMessage comCreationRes = await SendAuthenticatedPostAsync($"/posts/{createdPostId}/comments", token, postDTO);

            Assert.That(comCreationRes.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            JsonNode? comCreationMessage = await comCreationRes.ReadJsonAsync();

            int? createdCommentId = comCreationMessage?["data"]?["id"]?.GetValue<int>();
            Console.WriteLine($"Created comment id: {createdCommentId}|");

            Assert.That(createdCommentId, Is.Not.Null);


            CreatePostCommentDTO updateDTO = new ()  { Content = newContent };
            // Act 
            HttpResponseMessage response = await SendAuthenticatedPatchAsync($"/comments/{createdCommentId}", token, updateDTO);
            JsonNode? message = await response.ReadJsonAsync();

            Console.WriteLine(message);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(expectedStatus));

            if (expectedStatus == HttpStatusCode.OK)
            {
                JsonNode? updateMessage = await response.ReadJsonAsync();
                var data = updateMessage?["data"];
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(data?["content"]?.GetValue<string>(), Is.EqualTo(newContent));
                    Assert.That(data?["updatedAt"], Is.Not.Null);
                }
            }
        }

        [Test]
        public async Task EditComment_NotFound()
        {

            string token = await LoginAndGetToken("oyvind.perez1@example.com", "SuperHash!4");

            // Arrange
            int nonExistentCommentId = 99999;
            var updateDto = new CreatePostCommentDTO { Content = "This will fail" };

            // Act
            HttpResponseMessage response = await SendAuthenticatedPatchAsync($"/comments/{nonExistentCommentId}", token, updateDto);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        //Test story:
        // Student creates a post and two comments on that post. 
        // Student deletes one of the comments, and tries to delete the same again
        // Teacher deletes the other comment and tries again
        public async Task DeleteComment_SuccessAndThenNotFound()
        {
            // Arrange tokens
            string studentToken = await LoginAndGetToken(StudentEmail1, StudentPassword1);
            string teacherToken = await LoginAndGetToken(TeacherEmail, TeacherPassword);

            // Arrange: Students post and two comments on that post
            CreatePostDTO postDTO = new() { Content = "MY COOL POST!!!!" };
            CreatePostCommentDTO commentDTO1 = new() { Content = "MY COOL COMMENT!!!!" };
            CreatePostCommentDTO commentDTO2 = new() { Content = "MY OTHER COOL COMMENT :D!!!!" };

            // Act 1: Create the posts
            HttpResponseMessage postres = await SendAuthenticatedPostAsync("/posts", studentToken, postDTO);
            JsonNode? postMessage = await postres.ReadJsonAsync();
            int? postId = postMessage?["data"]?["id"]?.GetValue<int>();

            HttpResponseMessage comres1 = await SendAuthenticatedPostAsync($"/posts/{postId}/comments", studentToken, postDTO);
            JsonNode? comMessage1 = await comres1.ReadJsonAsync();
            int? commentId1 = comMessage1?["data"]?["id"]?.GetValue<int>();

            HttpResponseMessage comres2 = await SendAuthenticatedPostAsync($"/posts/{postId}/comments", studentToken, postDTO);
            JsonNode? comMessage2 = await comres2.ReadJsonAsync();
            int? commentId2 = comMessage2?["data"]?["id"]?.GetValue<int>();

            Console.WriteLine($"PostId: {postId} | CommentId1: {commentId1} | CommentId2: {commentId2} |");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(postId, Is.Not.Null);
                Assert.That(commentId1, Is.Not.Null);
                Assert.That(commentId2, Is.Not.Null);
            }

            // Act 2: Student deletes a post and tries to delete the same
            HttpResponseMessage studDel1 = await SendAuthenticatedDeleteAsync($"/comments/{commentId1}", studentToken);
            Assert.That(studDel1.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            HttpResponseMessage studDel2 = await SendAuthenticatedDeleteAsync($"/comments/{commentId1}", studentToken);
            Assert.That(studDel2.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

            // Act 3: Teacher tries to delete and deletes again
            HttpResponseMessage teacDel1 = await SendAuthenticatedDeleteAsync($"/comments/{commentId2}", teacherToken);
            Assert.That(teacDel1.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            HttpResponseMessage teacDel2 = await SendAuthenticatedDeleteAsync($"/comments/{commentId2}", teacherToken);
            Assert.That(teacDel2.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }


        [Test]
        public async Task GetPostsByUser_Success()
        {
            //Assert get token
            var token = await LoginAndGetToken("oyvind.perez1@example.com", "SuperHash!4");

            // Arrange the request with token
            int userId = 3; // CHeck if this user still has posts if this test fails (manuially)

            //ACT
            HttpResponseMessage response = await SendAuthenticatedGetAsync($"/posts/user/{userId}", token);

            // Assert
            JsonNode? message = await response.ReadJsonAsync();
            Console.WriteLine(message);
            // Assert status
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            
            Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo("Success"));
            var data = message?["data"]?.AsArray();
            Assert.That(data, Is.Not.Null);
            Assert.That(data?.Count, Is.GreaterThan(0), "Expected user 1 to have at least one post.");

            // Assert that ALL posts in the response belong to the correct user
            foreach (var post in data)
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

            var token = await LoginAndGetToken("oyvind.perez1@example.com", "SuperHash!4");
            
            // ACT
            var response = await SendAuthenticatedGetAsync($"/posts/user/{nonExistentUserId}", token);


            //assert
            JsonNode? message = await response.ReadJsonAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo("No posts found for this user"));
            Assert.That(message?["data"], Is.Null);
        }

        [Test]
        public async Task GetCommentsByUser_Success()
        {

            //Assert get token
            var token = await LoginAndGetToken("oyvind.perez1@example.com", "SuperHash!4");

            // Arrange the request with token
            int userId = 4; // CHeck if this user still has comments if this test fails (manuially)

            // ACT
            HttpResponseMessage response = await SendAuthenticatedGetAsync($"/comments/user/{userId}", token);

            // Assert
            JsonNode? message = await response.ReadJsonAsync();
            Console.WriteLine(message);

            // Assert status
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo("Success"));
            var data = message?["data"]?.AsArray();
            Assert.That(data, Is.Not.Null);
            Assert.That(data?.Count, Is.GreaterThan(0), "Expected user 1 to have at least one comment.");

            // Assert that ALL comments in the response belong to the correct user
            foreach (var comment in data)
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

            var token = await LoginAndGetToken("oyvind.perez1@example.com", "SuperHash!4");
            //ACT
            HttpResponseMessage response = await SendAuthenticatedGetAsync($"/comments/user/{nonExistentUserId}", token);


            // Assert
            JsonNode? message = await response.ReadJsonAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo("No comments found for this user"));
            Assert.That(message?["data"], Is.Null);
        }

    }
}
