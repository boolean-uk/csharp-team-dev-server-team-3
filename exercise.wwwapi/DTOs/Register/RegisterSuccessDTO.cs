using System.ComponentModel.DataAnnotations.Schema;

namespace exercise.wwwapi.DTOs.Register
{
    [NotMapped]
    public class RegisterSuccessDTO
    {
        public UserDTO user { get; set; } = new UserDTO();
    }
}
