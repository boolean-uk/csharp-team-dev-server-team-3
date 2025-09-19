using exercise.wwwapi.DTOs.GetUsers;

namespace exercise.wwwapi.DTOs.Cohort
{
    public class CourseInCohortDTO
    {
        public string Title { get; set; }
        public ICollection<UserBasicDTO> Students { get; set; } = new List<UserBasicDTO>();
        public ICollection<UserBasicDTO> Teachers { get; set; } = new List<UserBasicDTO>();
        //public ICollection<UserCohortDTO> Students { get; set; } = new List<UserCohortDTO>();
        //public ICollection<UserCohortDTO> Teachers { get; set; } = new List<UserCohortDTO>();
    }
}
