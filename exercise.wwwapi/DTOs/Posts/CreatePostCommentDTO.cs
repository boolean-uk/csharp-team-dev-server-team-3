using exercise.wwwapi.DTOs.GetUsers;

namespace exercise.wwwapi.DTOs.Posts
{
    public class CreatePostCommentDTO
    {
        public required int Userid { get; set; }
        public required string Content { get; set; }
    }
}
