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
            IEnumerable<Post> results = service.GetWithIncludes(q => q.Include(p => p.User));
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
            Post? post = service.GetById(id, q => q.Include(p => p.User));
            if (post == null) return TypedResults.NotFound(new ResponseDTO<Object> { Message = "Post not found" });

            service.Delete(id);
            service.Save();

            return TypedResults.Ok(new ResponseDTO<PostDTO> { Message = "Success" });
        }
    }
}
