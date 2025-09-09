namespace exercise.wwwapi.DTOs
{
    public class UserDTO
    {
        public int id { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? bio { get; set; }
        public string? githubUsername { get; set; }
        public string? username { get; set; }
    }
}
