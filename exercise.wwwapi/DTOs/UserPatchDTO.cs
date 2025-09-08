using exercise.wwwapi.Models;

namespace exercise.wwwapi.DTOs
{
    public class UserPatchDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? GitHubUsername { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? Password { get; set; }
        public int? Role { get; set; }
        public string? Specialism { get; set; }
        public string? Cohort { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Bio { get; set; }
    }
}
