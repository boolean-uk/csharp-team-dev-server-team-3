namespace exercise.wwwapi.DTOs.Cohort
{
    public class BasicCohortDTO
    {
        public int Id { get; set; }
        public required string Title { get; init; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
