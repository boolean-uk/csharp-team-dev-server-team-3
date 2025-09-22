using exercise.wwwapi.Models;

namespace exercise.wwwapi.Data
{
    public class CohortCourseData
    {
        private List<string> _courseNames = new List<string>()
        {
            "Software Development", 
            "Front-End Development", 
            "Data Analytics"
        };
        private List<string> _cohortNames = new List<string>()
        {
            "Cohort 1",
            "Cohort 2",
            "Best Cohort (Nigel's Cohort)",
            "Cohort 4",
            "Cohort 5",
        };
        private List<Course> _courses = new List<Course>();
        private List<Cohort> _cohorts = new List<Cohort>();
        private List<CohortCourse> _cohortCourses = new List<CohortCourse>();
        private List<CohortCourseUser> _cohortCourseUsers = new List<CohortCourseUser>();
        public CohortCourseData(List<User> users) 
        { 
            Random random = new Random(1);

            for (int x = 0; x < _courseNames.Count; x++)
            {
                Course course = new Course() { Id = x+1 , Title = _courseNames[x] };
                _courses.Add(course);
            }

            for (int x = 0; x < _cohortNames.Count; x++)
            {
                Cohort cohort = new Cohort() { Id = x+1 , Title = _cohortNames[x] };
                _cohorts.Add(cohort);
            }

            foreach (var cohort in _cohorts)
            {
                foreach (var course in _courses)
                {
                    CohortCourse cc = new CohortCourse()
                    {
                        CohortId = cohort.Id,
                        CourseId = course.Id
                    };
                    _cohortCourses.Add(cc);
                }
            }

            foreach (var user in users)
            {
                var cc = _cohortCourses[random.Next(_cohortCourses.Count)];
                CohortCourseUser ccu = new CohortCourseUser()
                {
                    CohortId = cc.CohortId,
                    CourseId = cc.CourseId,
                    UserId = user.Id
                };
                _cohortCourseUsers.Add(ccu);
            }
        }
        public List<Course> Courses { get { return _courses; } }
        public List<Cohort> Cohorts { get { return _cohorts; } }
        public List<CohortCourse> CohortCourses { get { return _cohortCourses; } }
        public List<CohortCourseUser> CohortCourseUsers { get { return _cohortCourseUsers; } }

    }
}
