using exercise.wwwapi.DTOs.Cohort;
using exercise.wwwapi.Models;

namespace exercise.wwwapi.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? GithubUsername { get; set; }
        public string? Mobile { get; set; }
        public Roles? Role { get; set; }
        public string? Specialism { get; set; }
        public string? Bio { get; set; }
        public string? Photo { get; set; }
        public BasicCohortDTO? Cohort { get; set; }

    }
}
