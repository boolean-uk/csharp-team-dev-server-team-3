using exercise.wwwapi.DTOs.GetUsers;
using exercise.wwwapi.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace exercise.wwwapi.DTOs.Cohort
{
    public class CohortCourseUserDTO
    {
        public string Cohort { get; set; }
        public string Course { get; set; }
        public UserBasicDTO User { get; set; }
    }
}
