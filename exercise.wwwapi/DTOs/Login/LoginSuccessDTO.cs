using exercise.wwwapi.Models;

namespace exercise.wwwapi.DTOs.Login
{
    public class LoginSuccessDTO
    {
        public required string Token { get; set; }
        public required UserDTO User { get; set; } 
    }
}
