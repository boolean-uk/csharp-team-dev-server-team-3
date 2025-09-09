using System.ComponentModel.DataAnnotations.Schema;

namespace exercise.wwwapi.DTOs
{
    [NotMapped]
    public class UserResponseDto
    {
        public string username { get; set; }        
        public string email { get; set; }
        public string role { get; set; } = "STUDENT";
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string bio { get; set; }
        public string githubUsername { get; set; } 
    }
}
