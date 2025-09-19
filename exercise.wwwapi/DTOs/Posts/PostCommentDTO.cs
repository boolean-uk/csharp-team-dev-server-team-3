using exercise.wwwapi.DTOs.GetUsers;

namespace exercise.wwwapi.DTOs.Posts
{
    public class PostCommentDTO
    {
        public int Id { get; set; }
        public UserBasicDTO? User { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
