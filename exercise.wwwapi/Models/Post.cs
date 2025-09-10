using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace exercise.wwwapi.Models
{
    [Table("posts")]
    public class Post
    {
        [Key, Column("id")]
        public required int Id { get; set; }
        [Column("userid")]
        public required int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        [Column("content")]
        public string Content { get; set; } = string.Empty;
        [Column("createdat")]
        public required DateTime CreatedAt { get; set; }
        [Column("updatedat")]
        public DateTime? UpdatedAt { get; set; }
        [Column("numlikes")]
        public required int NumLikes { get; set; } = 0;

        public ICollection<PostComment> Comments { get; set; } = new List<PostComment>();
    }
}
