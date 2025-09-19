using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace exercise.wwwapi.Models
{
    public class CohortCourse
    {
        [Column("cohortid")]
        public int CohortId { get; set; }
        [ForeignKey("CohortId")]
        public Cohort Cohort { get; set; }
        [Column("courseid")]
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
        public ICollection<CohortCourseUser> CohortCourseUsers { get; set; } = new List<CohortCourseUser>();
    }
}
