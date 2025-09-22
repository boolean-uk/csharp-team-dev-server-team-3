using System.ComponentModel.DataAnnotations.Schema;

namespace exercise.wwwapi.Models
{
    public class CohortCourseUser
    {
        [Column("cohortid")]
        public int CohortId { get; set; }
        [ForeignKey("CohortId")]
        public Cohort Cohort { get; set; }
        [Column("courseid")]
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
        [Column("userid")]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        //public DateTime EnrolledAt { get; set; }
        //public string Status { get; set; } // Active, Completed, Dropped, etc.
    }
}
