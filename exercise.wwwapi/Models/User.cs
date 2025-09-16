using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace exercise.wwwapi.Models
{
    public enum Roles { student, teacher }
    [Table("users")]
    public class User
    {
        [Key,Column("id")]
        public int Id { get; set; }

        [Column("username")]
        public string Username { get; set; } = string.Empty;

        [Column("passwordhash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("email"), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Column("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [Column("lastName")]
        public string LastName { get; set; } = string.Empty;

        [Column("bio")]
        public string Bio { get; set; } = string.Empty;

        [Column("GithubUsername")]
        public string GithubUsername { get; set; } = string.Empty;

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

        [Column("photo")]
        public string Photo {  get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<UserCohort> UserCohorts { get; set; } = new List<UserCohort>();

    }
}
