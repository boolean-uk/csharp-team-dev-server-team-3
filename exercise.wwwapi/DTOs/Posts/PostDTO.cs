using exercise.wwwapi.DTOs.GetUsers;

namespace exercise.wwwapi.DTOs.Posts
{
    public class PostDTO
    {
        public required int Id { get; set; }
        public UserBasicDTO? User { get; set; }
        public required string Content { get; set; }
        public required DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public required int NumLikes { get; set; }

        public ICollection<PostCommentDTO> Comments { get; set; } = new List<PostCommentDTO>();
    }
}
