using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace exercise.wwwapi.Models
{
    public class Course
    {
        [Key, Column("id")]
        public int Id { get; set; }
        [Column("title")]
        public string Title { get; set; }
        public ICollection<CohortCourse> CohortCourses { get; set; } = new List<CohortCourse>();
        
    }
}
