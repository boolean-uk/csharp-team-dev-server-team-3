using System.ComponentModel.DataAnnotations.Schema;

namespace exercise.wwwapi.DTOs.Login
{
    [NotMapped]
    public class LoginRequestDTO
    {
        public string? email { get; set; }
        public string? password { get; set; }
        public bool? longlifetoken { get; set; }
    }
}
