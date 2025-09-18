using exercise.wwwapi.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace exercise.wwwapi.DTOs
{
    public class UserDTO
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? GithubUsername { get; set; }
        public string? Mobile { get; set; }
        public Roles? Role { get; set; }
        public string? Specialism { get; set; }
        public string? Bio { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Photo { get; set; }
    }
}
