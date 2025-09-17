using AutoMapper;
using exercise.wwwapi.Data;
using exercise.wwwapi.DTOs;
using exercise.wwwapi.DTOs.GetUsers;
using exercise.wwwapi.DTOs.Posts;
using exercise.wwwapi.Models;
using exercise.wwwapi.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace exercise.wwwapi.Endpoints
{
    public static class PostEndpoints
    {
        public static void ConfigurePostEndpoints(this WebApplication app)
        {
            var posts = app.MapGroup("posts");
            posts.MapPost("/", CreatePost).WithSummary("Create post");
            posts.MapGet("/", GetAllPosts).WithSummary("Get all posts");
            posts.MapPatch("/{id}", UpdatePost).WithSummary("Update a certain post");
            posts.MapDelete("/{id}", DeletePost).WithSummary("Remove a certain post");

            posts.MapPost("/{postId}/comments", AddCommentToPost).WithSummary("Add a new comment to a post");
            posts.MapGet("/{postId}/comments", GetCommentsForPost).WithSummary("Get comments for a specific post (with pagination)");

            // Standalone comment endpoints for editing/deleting
            var comments = app.MapGroup("comments");
            comments.MapPatch("/{id}", UpdateComment).WithSummary("Edit an existing comment");
            comments.MapDelete("/{id}", DeleteCommentById).WithSummary("Remove an existing comment");

        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public static IResult CreatePost(IRepository<User> userservice, IRepository<Post> postservice, IMapper mapper, CreatePostDTO request)
        {

            User? user = userservice.GetById(request.Userid);
            if (user == null)
                return Results.NotFound(new ResponseDTO<Object>{Message = "Invalid userID"});

            if (string.IsNullOrWhiteSpace(request.Content))
                return Results.BadRequest(new ResponseDTO<Object> { Message = "Content cannot be empty" });

            Post post = new Post() { CreatedAt = DateTime.UtcNow, NumLikes = 0, UserId=request.Userid, Content=request.Content };

            // is a try catch needed here?
            postservice.Insert(post);
            postservice.Save();


            UserBasicDTO userBasicDTO = mapper.Map<UserBasicDTO>(user);
            PostDTO postDTO = mapper.Map<PostDTO>(post);
            postDTO.User = userBasicDTO;

            ResponseDTO<PostDTO> response = new ResponseDTO<PostDTO>
            {
                Message = "success",
                Data = postDTO
            };

            return Results.Created($"/posts/{post.Id}", response);
        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        public static IResult GetAllPosts(IRepository<Post> service, IMapper mapper)
        {
            IEnumerable<Post> results = service.GetWithIncludes(q => q.Include(p => p.User).Include(p => p.Comments).ThenInclude(c => c.User));
            IEnumerable<PostDTO> postDTOs = mapper.Map<IEnumerable<PostDTO>>(results);
            ResponseDTO<IEnumerable<PostDTO>> response = new ResponseDTO<IEnumerable<PostDTO>>()
            {
                Message = "success",
                Data = postDTOs
            };
            return TypedResults.Ok(response);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public static IResult UpdatePost(IRepository<Post> service, IMapper mapper, int id, UpdatePostDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Content)) return TypedResults.BadRequest(new ResponseDTO<object>{
                    Message = "Content cannot be empty"
                });
            
            Post? post = service.GetById(id, q=>q.Include(p => p.User));

            if (post == null) return TypedResults.NotFound(new ResponseDTO<Object> { Message = "Post not found" });

            post.Content = request.Content;
            post.UpdatedAt = DateTime.UtcNow;

            service.Update(post);
            service.Save();

            PostDTO postDTO = mapper.Map<PostDTO>(post);


            return TypedResults.Ok(new ResponseDTO<PostDTO> { Message = "Success", Data = postDTO });
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        private static IResult DeletePost(IRepository<Post> service, int id)
        {
            Post? post = service.GetById(id, q => q.Include(p => p.User).Include(p => p.Comments).ThenInclude(c => c.User));
            if (post == null) return TypedResults.NotFound(new ResponseDTO<Object> { Message = "Post not found" });

            service.Delete(id);
            service.Save();

            return TypedResults.Ok(new ResponseDTO<PostDTO> { Message = "Success" });
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        private static IResult AddCommentToPost(IRepository<PostComment> commentService, IRepository<Post> postService, IRepository<User> userService, IMapper mapper, int postId, CreatePostCommentDTO request)
        {
            // Check if post exists
            var post = postService.GetById(postId);
            if (post == null)
            {
                return TypedResults.NotFound(new ResponseDTO<object> { Message = "Post not found." });
            }

            // Check if user exists
            var user = userService.GetById(request.Userid);
            if (user == null)
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
                UserId = request.Userid,
                PostId = postId,
                CreatedAt = DateTime.UtcNow
            };

            commentService.Insert(comment);
            commentService.Save();

            var createdComment = commentService.GetById(comment.Id, q => q.Include(c => c.User));
            var commentDto = mapper.Map<PostCommentDTO>(createdComment);

            return TypedResults.Created($"/comments/{comment.Id}", new ResponseDTO<PostCommentDTO> { Message = "Success", Data = commentDto });
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        private static IResult GetCommentsForPost(IRepository<PostComment> service, IMapper mapper, int postId)
        {
            PostComment? comment = service.GetById(postId, q => q.Include(c => c.User));
            if (comment == null) return TypedResults.NotFound(new ResponseDTO<Object> { Message = "Comment not found" });
            PostCommentDTO commentDTO = mapper.Map<PostCommentDTO>(comment);
            return TypedResults.Ok(new ResponseDTO<PostCommentDTO> { Message = "Success", Data = commentDTO });
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        private static IResult UpdateComment(IRepository<PostComment> service, IMapper mapper, int id, CreatePostCommentDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return TypedResults.BadRequest(new ResponseDTO<object> { Message = "Content cannot be empty." });
            }

            var comment = service.GetById(id, q => q.Include(c => c.User));
            if (comment == null)
            {
                return TypedResults.NotFound(new ResponseDTO<object> { Message = "Comment not found." });
            }

            comment.Content = request.Content;
            comment.UpdatedAt = DateTime.UtcNow;

            service.Update(comment);
            service.Save();

            var commentDto = mapper.Map<PostComment>(comment);
            return TypedResults.Ok(new ResponseDTO<PostComment> { Message = "Comment updated successfully.", Data = commentDto });
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        private static IResult DeleteCommentById(IRepository<PostComment> service, int id)
        {
            var comment = service.GetById(id);
            if (comment == null)
            {
                return TypedResults.NotFound(new ResponseDTO<object> { Message = "Comment not found." });
            }

            service.Delete(id);
            service.Save();

            return TypedResults.Ok(new ResponseDTO<object> { Message = "Comment deleted successfully." });
        }
    }
}
