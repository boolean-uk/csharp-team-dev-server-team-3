namespace exercise.wwwapi.DTOs
{
    public class UserPatchDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? GithubUsername { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? Password { get; set; }
        public int? Role { get; set; }
        public string? Specialism { get; set; }
        public string? Cohort { get; set; }
        public string? Bio { get; set; }
        public string? Photo { get; set; }
    }
}
