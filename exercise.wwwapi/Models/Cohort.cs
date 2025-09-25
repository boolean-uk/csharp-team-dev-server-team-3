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
        [Column("startDate")]
        public DateTime StartDate { get; set; }

        [Column("endDate")]
        public DateTime EndDate { get; set; }
        [JsonIgnore]
        public ICollection<CohortCourse> CohortCourses { get; set; } = new List<CohortCourse>();
    }
}