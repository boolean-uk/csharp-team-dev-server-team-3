using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace exercise.wwwapi.Models
{
    [Table("comments")]
    public class PostComment
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Column("postid")]
        public int PostId { get; set; }

        [ForeignKey("PostId")]
        public Post? Post { get; set; }

        [Column("userid")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Column("content")]
        public string Content { get; set; } = string.Empty;

        [Column("createdat")]
        public DateTime CreatedAt { get; set; }

        [Column("updatedat")]
        public DateTime? UpdatedAt { get; set; }
    }
}
