namespace exercise.wwwapi.DTOs.Cohort
{
    public class CohortDTO
    {
        public string Title { get; set; }

        public ICollection<UserCohortDTO> Students { get; set; } = new List<UserCohortDTO>();

        public ICollection<UserCohortDTO> Teachers { get; set; } = new List<UserCohortDTO>();
    }
}
