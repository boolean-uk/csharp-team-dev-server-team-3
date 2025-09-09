using System.ComponentModel.DataAnnotations.Schema;

namespace exercise.wwwapi.DTOs.Register
{
    [NotMapped]
    public class RegisterRequestDTO
    {
        public required string email { get; set; }
        public required string password { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? bio { get; set; }
        public string? githubUsername { get; set; }
        public string? username { get; set; } 
    }
}
