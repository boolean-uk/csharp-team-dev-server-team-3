using exercise.wwwapi.DTOs.GetUsers;

namespace exercise.wwwapi.DTOs.Cohort
{
    public class CohortDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<CourseInCohortDTO> Courses { get; set; } = new List<CourseInCohortDTO>();
    }
}
