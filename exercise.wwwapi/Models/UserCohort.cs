using System.ComponentModel.DataAnnotations.Schema;

namespace exercise.wwwapi.Models
{
    [Table("userCohorts")]
    public class UserCohort
    {
        [Column("cohortid")]
        public required int CohortId { get; set; }
        [ForeignKey("CohortId")]
        public Cohort? Cohort { get; set; }


        [Column("userid")]
        public required int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
