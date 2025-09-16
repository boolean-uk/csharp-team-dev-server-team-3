using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace exercise.wwwapi.Models
{
    [Table("cohorts")]
    public class Cohort
    {
        [Key, Column("id")]
        public int Id { get; set; }
        [Column("title")]
        public string Title { get; set; } = string.Empty;
        //[Column("userid")]
        //public int UserId { get; set; }
        //[ForeignKey("UserId")]
        //public User? User { get; set; }
        [JsonIgnore]
        public ICollection<UserCohort> UserCohorts { get; set; } = new List<UserCohort>();
    }
}
