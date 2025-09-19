//using AutoMapper;
//using exercise.wwwapi.DTOs.Cohort;
//using exercise.wwwapi.Models;
//using exercise.wwwapi.Repository;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace exercise.wwwapi.Endpoints
//{
//    public static class CohortEndpoints_old
//    {
//        public static void ConfigureCohortEndpoints(this WebApplication app)
//        {
//            var cohorts = app.MapGroup("cohorts");
//            cohorts.MapPost("/", CreateCohort).WithSummary("Create a cohort");
//            cohorts.MapGet("/", GetAllCohorts).WithSummary("Get all cohorts");
//            //cohorts.MapGet("/{id}", GetCohort).WithSummary("Get a cohort by ID");
//            cohorts.MapPost("/{cohortId}/{userId}/{courseId}", AddUserToCohort).WithSummary("Add a user to a cohort");
//            cohorts.MapDelete("/{cohortId}/{userId}/{courseId}", DeleteUserFromCohort).WithSummary("Delete a user from a cohort");
//        }
//        //[ProducesResponseType(StatusCodes.Status200OK)]
//        //public static async Task<IResult> GetCohort(IRepository<Cohort> service, IMapper mapper, int cohortId)
//        //{
//        //    //var result = service.GetById(cohortId);
//        //    var result = service.GetById(cohortId, q => q.Include(c => c.UserCohorts).ThenInclude(cu => cu.User));
//        //    CohortDTO cohortDTOs = mapper.Map<CohortDTO>(result);

//        //    return TypedResults.Ok(cohortDTOs);
//        //}

//        [ProducesResponseType(StatusCodes.Status200OK)]
//        public static async Task<IResult> GetAllCohorts(IRepository<Cohort> service, IMapper mapper)
//        {
//            var results = service.GetWithIncludes(q => q
//                .Include(c => c.CohortCourses)
//                    .ThenInclude(cc => cc.Course)                   
//                .Include(c => c.CohortCourses)
//                    .ThenInclude(cc => cc.CohortCourseUsers)      
//                        .ThenInclude(ccu => ccu.User)      
//            );

//            IEnumerable<CohortDTO> cohortDTOs = mapper.Map<IEnumerable<CohortDTO>>(results);

//            return TypedResults.Ok(cohortDTOs);;
//        }


//        [ProducesResponseType(StatusCodes.Status201Created)]
//        public static async Task<IResult> CreateCohort(
//            IRepository<Cohort> cohortRepo,
//            IRepository<Course> courseRepo,
//            IMapper mapper,
//            CreateCohortDTO request)
//        {
//            var cohort = new Cohort { Title = request.Title };

//            string[] defaultCourses = { "Software Development", "Front-End Development", "Data Analytics" };

//            var courses = new List<Course>();
//            foreach (var courseTitle in defaultCourses)
//            {
//                var course = courseRepo.Table.FirstOrDefault(c => c.Title == courseTitle);
//                if (course == null)
//                {
//                    course = new Course { Title = courseTitle };
//                    courseRepo.Insert(course);
//                }
//                courses.Add(course);
//            }

//            courseRepo.Save();

//            cohort.CohortCourses = courses.Select(c => new CohortCourse
//            {
//                CourseId = c.Id,
//                Course = c,
//                Cohort = cohort
//            }).ToList();

//            cohortRepo.Insert(cohort);
//            cohortRepo.Save();

//            var response = mapper.Map<CohortDTO>(cohort);
//            return TypedResults.Ok(response);
//        }

//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        public static async Task<IResult> AddUserToCohort(
//            IRepository<Cohort> cohortRepo,
//            IRepository<User> userRepo,
//            IRepository<UserCourse> userCourseRepo,
//            IRepository<CohortCourseUser> cohortCourseUserRepo,
//            IMapper mapper,
//            int userId,
//            int cohortId,
//            int courseId)
//        {
//            // 1. Get the user
//            var user = userRepo.GetById(userId);
//            if (user == null) return Results.NotFound($"User with Id {userId} not found.");

//            // 2. Get the cohort including its users and courses for verification steps
//            var cohort = cohortRepo.GetById(cohortId, q =>
//                q.Include(c => c.UserCohorts)
//                 .Include(c => c.CohortCourses)
//                    .ThenInclude(cc => cc.Course));
//            if (cohort == null) return Results.NotFound($"Cohort with Id {cohortId} not found.");

//            // 3. Verify that the course exists in this cohort
//            var cohortCourse = cohort.CohortCourses.FirstOrDefault(cc => cc.CourseId == courseId);
//            if (cohortCourse == null)
//                return Results.BadRequest("The specified course is not part of this cohort.");

//            // 4. Check if the user is already in this cohort
//            if (cohort.UserCohorts.Any(cu => cu.UserId == userId))
//                return Results.BadRequest("User is already a member of this cohort.");

//            // 5. Add user to the cohort
//            var userCohort = new UserCohort
//            {
//                CohortId = cohortId,
//                UserId = userId,
//                Cohort = cohort,
//                User = user
//            };
//            cohort.UserCohorts.Add(userCohort);

//            // 6. Enroll user in the course (UserCourse)
//            var existingEnrollment = userCourseRepo
//                .GetAllFiltered(uc => uc.UserId == userId && uc.CourseId == courseId)
//                .FirstOrDefault();

//            if (existingEnrollment != null)
//                return Results.BadRequest("User is already enrolled in the course.");

//            var userCourse = new UserCourse
//            {
//                UserId = userId,
//                User = user,
//                CourseId = courseId,
//                Course = cohortCourse.Course,
//                EnrolledAt = DateTime.UtcNow,
//                //Active = true
//            };
//            userCourseRepo.Insert(userCourse);

//            // 7. Add user to CohortCourseUser
//            var existingCcu = cohortCourseUserRepo
//                .GetAllFiltered(ccu => ccu.UserId == userId && ccu.CohortId == cohortId && ccu.CourseId == courseId)
//                .FirstOrDefault();

//            if (existingCcu == null)
//            {
//                var ccu = new CohortCourseUser
//                {
//                    UserId = userId,
//                    User = user,
//                    CohortId = cohortId,
//                    Cohort = cohort,
//                    CourseId = courseId,
//                    Course = cohortCourse.Course
//                };
//                cohortCourseUserRepo.Insert(ccu);

//                // Optional: also attach to navigation property so EF tracks it
//                if (cohortCourse.CohortCourseUsers == null)
//                    cohortCourse.CohortCourseUsers = new List<CohortCourseUser>();
//                cohortCourse.CohortCourseUsers.Add(ccu);
//            }

//            // 8. Save changes
//            cohortRepo.Save();
//            userCourseRepo.Save();
//            cohortCourseUserRepo.Save();

//            // 9. Map response
//            var response = mapper.Map<UserCohortDTO>(userCohort);
//            return TypedResults.Ok(response);
//        }

//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        public static async Task<IResult> DeleteUserFromCohort(
//            IRepository<Cohort> cohortRepo,
//            IRepository<User> userRepo,
//            IRepository<UserCourse> userCourseRepo,
//            IRepository<CohortCourseUser> cohortCourseUserRepo,
//            IMapper mapper,
//            int userId,
//            int cohortId,
//            int courseId)
//        {
//            // 1. Get the user
//            var user = userRepo.GetById(userId);
//            if (user == null) return Results.NotFound($"User with Id {userId} not found.");

//            // 2. Get the cohort including its users and courses for verification steps
//            var cohort = cohortRepo.GetById(cohortId, q =>
//                q.Include(c => c.UserCohorts)
//                 .Include(c => c.CohortCourses)
//                    .ThenInclude(cc => cc.Course));
//            if (cohort == null) return Results.NotFound($"Cohort with Id {cohortId} not found.");

//            // 3. Verify that the course exists in this cohort
//            var cohortCourse = cohort.CohortCourses.FirstOrDefault(cc => cc.CourseId == courseId);
//            if (cohortCourse == null)
//                return Results.BadRequest("The specified course is not part of this cohort.");

//            // 4. Check if the user is already in this cohort
//            if (!cohort.UserCohorts.Any(cu => cu.UserId == userId))
//                return Results.BadRequest("User is not a member of this cohort.");

//            var uc = cohort.UserCohorts.FirstOrDefault(cu => cu.UserId == userId);
//            cohort.UserCohorts.Remove(uc);

//            // 5. Add user to the cohort
//            //var userCohort = new UserCohort
//            //{
//            //    CohortId = cohortId,
//            //    UserId = userId,
//            //    Cohort = cohort,
//            //    User = user
//            //};
//            //cohort.UserCohorts.Add(userCohort);

//            // 6. Enroll user in the course (UserCourse)
//            var existingEnrollment = userCourseRepo
//                .GetAllFiltered(uc => uc.UserId == userId && uc.CourseId == courseId)
//                .FirstOrDefault();

//            if (existingEnrollment == null)
//                return Results.BadRequest("User is not enrolled in the course.");

//            userCourseRepo.Delete(existingEnrollment.UserId, existingEnrollment.CourseId);

//            //var userCourse = new UserCourse
//            //{
//            //    UserId = userId,
//            //    User = user,
//            //    CourseId = courseId,
//            //    Course = cohortCourse.Course,
//            //    EnrolledAt = DateTime.UtcNow,
//            //    //Active = true
//            //};
//            //userCourseRepo.Insert(userCourse);

//            // 7. Add user to CohortCourseUser
//            var existingCcu = cohortCourseUserRepo
//                .GetAllFiltered(ccu => ccu.UserId == userId && ccu.CohortId == cohortId && ccu.CourseId == courseId)
//                .FirstOrDefault();

//            if (existingCcu != null)
//            {
//                cohortCourseUserRepo.Delete(existingCcu.CohortId, existingCcu.CourseId, existingCcu.UserId);
//                //var ccu = new CohortCourseUser
//                //{
//                //    UserId = userId,
//                //    User = user,
//                //    CohortId = cohortId,
//                //    Cohort = cohort,
//                //    CourseId = courseId,
//                //    Course = cohortCourse.Course
//                //};
//                //cohortCourseUserRepo.Insert(ccu);

//                //// Optional: also attach to navigation property so EF tracks it
//                //if (cohortCourse.CohortCourseUsers == null)
//                //    cohortCourse.CohortCourseUsers = new List<CohortCourseUser>();
//                //cohortCourse.CohortCourseUsers.Add(ccu);
//            }

//            // 8. Save changes
//            cohortRepo.Save();
//            userCourseRepo.Save();
//            cohortCourseUserRepo.Save();

//            // 9. Map response
//            var response = mapper.Map<UserCohortDTO>(uc);
//            return TypedResults.Ok(response);
//        }


//    }
//}
