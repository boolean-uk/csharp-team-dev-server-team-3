namespace exercise.wwwapi.DTOs.Cohort
{
    public class CohortDTO
    {
        public string Title { get; set; }

        public ICollection<UserCohortDTO> UserCohorts { get; set; } = new List<UserCohortDTO>();
    }
}
