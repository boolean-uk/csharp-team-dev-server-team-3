using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace exercise.wwwapi.Models
{
    public enum Roles { teacher, student } // Changed to public
    [Table("users")]
    public class User
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("username")]
        public string Username { get; set; }

        [Column("passwordhash")]
        public string PasswordHash { get; set; }

        [Column("email"), EmailAddress]
        public string Email { get; set; }

        [Column("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [Column("lastName")]
        public string LastName { get; set; } = string.Empty;

        [Column("bio")]
        public string Bio { get; set; } = string.Empty;

        [Column("githubUrl")]
        public string GithubUrl { get; set; } = string.Empty;

        [Column("mobile")]
        public string Mobile { get; set; } = string.Empty;

        [Column("role"), EnumDataType(typeof(Roles))]
        public Roles Role { get; set; }

        [Column("specialism")]
        public string Specialism { get; set; } = string.Empty;

        [Column("startDate")]
        public DateTime StartDate { get; set; }

        [Column("endDate")]
        public DateTime EndDate { get; set; }

        //public ICollection<Cohort> Cohorts { get; set; }

    }
}
