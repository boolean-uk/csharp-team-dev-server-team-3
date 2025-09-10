using System.ComponentModel.DataAnnotations.Schema;

namespace exercise.wwwapi.DTOs.Register
{
    [NotMapped]
    public class RegisterRequestDTO
    {
        public required string email { get; set; }
        public required string password { get; set; }
    }
}
