using AutoMapper;
using exercise.wwwapi.DTOs;
using exercise.wwwapi.DTOs.GetUsers;
using exercise.wwwapi.DTOs.Posts;
using exercise.wwwapi.Helpers;
using exercise.wwwapi.Models;
using exercise.wwwapi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace exercise.wwwapi.Endpoints
{
    public static class PostEndpoints
    {
        public static void ConfigurePostEndpoints(this WebApplication app)
        {
            var posts = app.MapGroup("posts");
            posts.MapPost("/", CreatePost).WithSummary("Create post");
            posts.MapGet("/", GetAllPosts).WithSummary("Get all posts");
            posts.MapPatch("/{postid}", UpdatePost).WithSummary("Update a certain post");
            posts.MapDelete("/{postid}", DeletePost).WithSummary("Remove a certain post");

            posts.MapPost("/{postId}/comments", AddCommentToPost).WithSummary("Add a new comment to a post");
            posts.MapGet("/{postId}/comments", GetCommentsForPost).WithSummary("Get comments for a specific post");

            // Standalone comment endpoints for editing/deleting
            var comments = app.MapGroup("comments");
            comments.MapPatch("/{commentId}", UpdateComment).WithSummary("Edit an existing comment");
            comments.MapDelete("/{commentId}", DeleteCommentById).WithSummary("Remove an existing comment");

            // Endpoints to get by user
            posts.MapGet("/user/{userId}", GetPostsByUser).WithSummary("Get posts by a specific user");
            comments.MapGet("/user/{userId}", GetCommentsByUser).WithSummary("Get comments by a specific user");
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        private static IResult CreatePost(
            IRepository<User> userservice, 
            IRepository<Post> postservice, 
            IMapper mapper, 
            ClaimsPrincipal user,
            CreatePostDTO request
            )
        {
            User? dbUser = userservice.GetById(user.UserRealId());
            if (dbUser == null) return Results.NotFound(new ResponseDTO<Object> { Message = "Invalid userID" });

            if (string.IsNullOrWhiteSpace(request.Content))
                return Results.BadRequest(new ResponseDTO<Object> { Message = "Content cannot be empty" });

            Post post = new Post() { CreatedAt = DateTime.UtcNow, NumLikes = 0, UserId= dbUser.Id, Content=request.Content };

            // is a try catch needed here?
            postservice.Insert(post);
            postservice.Save();


            UserBasicDTO userBasicDTO = mapper.Map<UserBasicDTO>(dbUser);
            PostDTO postDTO = mapper.Map<PostDTO>(post);
            postDTO.User = userBasicDTO;

            ResponseDTO<PostDTO> response = new ResponseDTO<PostDTO>
            {
                Message = "Success",
                Data = postDTO
            };

            return Results.Created($"/posts/{post.Id}", response);
        }
        
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        private static IResult GetAllPosts(IRepository<Post> service, IMapper mapper)
        {
            IEnumerable<Post> results = service.GetWithIncludes(q => q.Include(p => p.User).Include(p => p.Comments).ThenInclude(c => c.User));
            IEnumerable<PostDTO> postDTOs = mapper.Map<IEnumerable<PostDTO>>(results);
            ResponseDTO<IEnumerable<PostDTO>> response = new ResponseDTO<IEnumerable<PostDTO>>()
            {
                Message = "Success",
                Data = postDTOs
            };
            return TypedResults.Ok(response);
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]

        private static IResult UpdatePost(IRepository<Post> service, IMapper mapper, ClaimsPrincipal user, int postid, UpdatePostDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Content)) return TypedResults.BadRequest(new ResponseDTO<object>{
                    Message = "Content cannot be empty"
                });
            
            Post? post = service.GetById(postid, q=>q.Include(p => p.User));

            if (post == null) return TypedResults.NotFound(new ResponseDTO<Object> { Message = "Post not found" });

            Console.WriteLine($"Role:{user.Role()} {Roles.student}| {post.UserId} {user.UserRealId()}");
            if (post.UserId != user.UserRealId() && user.Role() == (int)Roles.student)
            {
                var forbiddenResponse = new ResponseDTO<object>
                {
                    Message = "You are not authorized to edit this post."
                };
                return TypedResults.Json(forbiddenResponse, statusCode: StatusCodes.Status403Forbidden);
            }


            post.Content = request.Content;
            post.UpdatedAt = DateTime.UtcNow;

            service.Update(post);
            service.Save();

            PostDTO postDTO = mapper.Map<PostDTO>(post);


            return TypedResults.Ok(new ResponseDTO<PostDTO> { Message = "Success", Data = postDTO });
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        private static IResult DeletePost(IRepository<Post> service, ClaimsPrincipal user, int postid)
        {
            Post? post = service.GetById(postid, q => q.Include(p => p.User).Include(p => p.Comments).ThenInclude(c => c.User));
            if (post == null) return TypedResults.NotFound(new ResponseDTO<Object> { Message = "Post not found" });

            if (user.Role() == (int)Roles.student && post.UserId != user.UserRealId())
            {
                var forbiddenResponse = new ResponseDTO<object>
                {
                    Message = "You are not authorized to delete this post."
                };
                return TypedResults.Json(forbiddenResponse, statusCode: StatusCodes.Status403Forbidden);
            }
            service.Delete(postid);
            service.Save();

            return TypedResults.Ok(new ResponseDTO<PostDTO> { Message = "Success" });
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        private static IResult AddCommentToPost(
            IRepository<PostComment> commentService, 
            IRepository<Post> postService, 
            IRepository<User> userService, 
            IMapper mapper, 
            ClaimsPrincipal user,
            int postId, 
            CreatePostCommentDTO request)
        {
            // Check if post exists
            var post = postService.GetById(postId);
            if (post == null)
            {
                return TypedResults.NotFound(new ResponseDTO<object> { Message = "Post not found." });
            }

            // Check if user exists
            int? userid = user.UserRealId();
            if (userid == null) return Results.NotFound(new ResponseDTO<Object> { Message = "UserId missing" });
            var dbUser = userService.GetById(userid);
            if (dbUser == null)
            {
                return TypedResults.NotFound(new ResponseDTO<object> { Message = "User not found." });
            }

            // Validate content
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return TypedResults.BadRequest(new ResponseDTO<object> { Message = "Comment content cannot be empty." });
            }

            var comment = new PostComment
            {
                Content = request.Content,
                UserId = dbUser.Id,
                PostId = postId,
                CreatedAt = DateTime.UtcNow
            };

            commentService.Insert(comment);
            commentService.Save();

            var createdComment = commentService.GetById(comment.Id, q => q.Include(c => c.User));
            var commentDto = mapper.Map<PostCommentDTO>(createdComment);

            return TypedResults.Created($"/comments/{comment.Id}", new ResponseDTO<PostCommentDTO> { Message = "Success", Data = commentDto });
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        private static IResult GetCommentsForPost(IRepository<Post> postservice, IMapper mapper, int postId)
        {
            Post? post = postservice.GetById(postId, q => q.Include(p => p.User).Include(p => p.Comments).ThenInclude(c => c.User));
            if (post == null) return TypedResults.NotFound(new ResponseDTO<Object> { Message = "Post not found" });
            List<PostComment> comments = [.. post.Comments];
            List<PostCommentDTO> commentsDTO = mapper.Map<List<PostCommentDTO>>(comments);
            return TypedResults.Ok(new ResponseDTO<List<PostCommentDTO>> { Message = "Success", Data = commentsDTO });
        }
        
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        private static IResult UpdateComment(
            IRepository<PostComment> service, 
            IMapper mapper, 
            ClaimsPrincipal user, 
            int commentId, 
            CreatePostCommentDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return TypedResults.BadRequest(new ResponseDTO<object> { Message = "Content cannot be empty." });
            }

            var comment = service.GetById(commentId, q => q.Include(c => c.User));
            if (comment == null) return TypedResults.NotFound(new ResponseDTO<object> { Message = "Comment not found." });

            //Console.WriteLine($"Role:{user.Role()} {Roles.student.ToString()}| {comment.UserId} {user.UserRealId()}");
            if (comment.UserId != user.UserRealId() && user.Role() == (int)Roles.student)
            {
                var forbiddenResponse = new ResponseDTO<object>
                {
                    Message = "You are not authorized to edit this comment."
                };
                return TypedResults.Json(forbiddenResponse, statusCode: StatusCodes.Status403Forbidden);
            }

            comment.Content = request.Content;
            comment.UpdatedAt = DateTime.UtcNow;

            service.Update(comment);
            service.Save();

            var commentDto = mapper.Map<PostComment>(comment);
            return TypedResults.Ok(new ResponseDTO<PostComment> { Message = "Comment updated successfully.", Data = commentDto });
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        private static IResult DeleteCommentById(IRepository<PostComment> service, ClaimsPrincipal user, int commentId)
        {
            var comment = service.GetById(commentId);
            if (comment == null)
            {
                return TypedResults.NotFound(new ResponseDTO<object> { Message = "Comment not found." });
            }

            if (user.Role() == (int)Roles.student && comment.UserId != user.UserRealId())
            {
                var forbiddenResponse = new ResponseDTO<object>
                {
                    Message = "You are not authorized to delete this comment."
                };
                return TypedResults.Json(forbiddenResponse, statusCode: StatusCodes.Status403Forbidden);
            }

            service.Delete(commentId);
            service.Save();

            return TypedResults.Ok(new ResponseDTO<object> { Message = "Comment deleted successfully." });
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        private static IResult GetPostsByUser(IRepository<Post> service, IMapper mapper, int userid)
        {
            IEnumerable<Post> results = service.GetWithIncludes(q => q.Where(p => p.UserId == userid).Include(p => p.User).Include(p => p.Comments).ThenInclude(c => c.User));
            if (results.Count() == 0) return TypedResults.NotFound(new ResponseDTO<Object> { Message = "No posts found for this user" });

            IEnumerable<PostDTO> postDTOs = mapper.Map<IEnumerable<PostDTO>>(results);
            ResponseDTO<IEnumerable<PostDTO>> response = new ResponseDTO<IEnumerable<PostDTO>>()
            {
                Message = "Success",
                Data = postDTOs
            };
            return TypedResults.Ok(response);
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        private static IResult GetCommentsByUser(IRepository<PostComment> service, IMapper mapper, int userid)
        {
            IEnumerable<PostComment> results = service.GetWithIncludes(q => q.Where(p => p.UserId == userid).Include(p => p.User));
            if (results.Count() == 0) return TypedResults.NotFound(new ResponseDTO<Object> { Message = "No comments found for this user" });

            IEnumerable<PostCommentDTO> PostCommentDTOs = mapper.Map<IEnumerable<PostCommentDTO>>(results);
            ResponseDTO<IEnumerable<PostCommentDTO>> response = new ResponseDTO<IEnumerable<PostCommentDTO>>()
            {
                Message = "Success",
                Data = PostCommentDTOs
            };
            return TypedResults.Ok(response);
        }
    }
}
