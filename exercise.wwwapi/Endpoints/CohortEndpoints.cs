using AutoMapper;
using exercise.wwwapi.DTOs;
using exercise.wwwapi.DTOs.Cohort;
using exercise.wwwapi.DTOs.Posts;
using exercise.wwwapi.Models;
using exercise.wwwapi.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace exercise.wwwapi.Endpoints
{
    public static class CohortEndpoints
    {
        public static void ConfigureCohortEndpoints(this WebApplication app)
        {
            var cohorts = app.MapGroup("cohorts");
            cohorts.MapPost("/", CreateCohort).WithSummary("Create a cohort");
            cohorts.MapGet("/", GetAllCohorts).WithSummary("Get all cohorts");
            cohorts.MapGet("/{id}", GetCohort).WithSummary("Get a cohort by ID");
            cohorts.MapPost("/{cohortId}/{userId}/{courseId}", AddUserToCohort).WithSummary("Add a user to a cohort");
            cohorts.MapDelete("/{cohortId}/{userId}/{courseId}", DeleteUserFromCohort).WithSummary("Delete a user from a cohort");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        public static async Task<IResult> GetCohort(IRepository<Cohort> service, IMapper mapper, int cohortId)
        {
            var result = service.GetById(cohortId, q => q
                .Include(c => c.CohortCourses)
                    .ThenInclude(cc => cc.Course)
                .Include(c => c.CohortCourses)
                    .ThenInclude(cc => cc.CohortCourseUsers)
                        .ThenInclude(ccu => ccu.User));
            CohortDTO cohortDTO = mapper.Map<CohortDTO>(result);
            ResponseDTO<CohortDTO> response = new ResponseDTO<CohortDTO>()
            {
                Message = "Success",
                Data = cohortDTO
            };

            return TypedResults.Ok(response);

            //return TypedResults.Ok(cohortDTOs);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        public static async Task<IResult> GetAllCohorts(IRepository<Cohort> cohortService, IMapper mapper)
        {
            var results = cohortService.GetWithIncludes(q => q
                .Include(c => c.CohortCourses)
                    .ThenInclude(cc => cc.Course)
                .Include(c => c.CohortCourses)
                    .ThenInclude(cc => cc.CohortCourseUsers)
                        .ThenInclude(ccu => ccu.User)
            );
            Console.WriteLine(results);

            IEnumerable<CohortDTO> cohortDTOs = mapper.Map<IEnumerable<CohortDTO>>(results);
            ResponseDTO<IEnumerable<CohortDTO>> response = new ResponseDTO<IEnumerable<CohortDTO>>()
            {
                Message = "Success",
                Data = cohortDTOs
            };

            return TypedResults.Ok(response);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        public static async Task<IResult> CreateCohort(
            IRepository<Cohort> cohortService,
            IRepository<Course> courseService,
            IMapper mapper,
            CreateCohortDTO request)
        {

            var results = cohortService.GetAllFiltered(c => c.Title == request.Title);
            if (results != null) return TypedResults.BadRequest(new ResponseDTO<object>
            {
                Message = $"Cohort with name {request.Title} already exists"
            });

            Cohort cohort = new Cohort() { Title = request.Title };

            string[] defaultCourses = { "Software Development", "Front-End Development", "Data Analytics" };
            var courses = new List<Course>();
            foreach (var courseTitle in defaultCourses)
            {
                var course = courseService.Table.FirstOrDefault(c => c.Title == courseTitle);
                if (course == null)
                {
                    course = new Course { Title = courseTitle };
                    courseService.Insert(course);
                }
                courses.Add(course);
            }
            courseService.Save();

            cohort.CohortCourses = courses.Select(c => new CohortCourse
            {
                CourseId = c.Id,
                Course = c,
                CohortId = cohort.Id,
                Cohort = cohort
            }).ToList();

            cohortService.Insert(cohort);
            cohortService.Save();

            var cohortDTO = mapper.Map<CohortDTO>(cohort);
            ResponseDTO<CohortDTO> response = new ResponseDTO<CohortDTO>()
            {
                Message = "Success",
                Data = cohortDTO
            };
            return TypedResults.Created($"/api/cohorts/{cohort.Id}", response);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public static async Task<IResult> AddUserToCohort(
                    IRepository<Cohort> cohortService,
                    IRepository<User> userService,
                    IRepository<CohortCourse> cohortCourseService,
                    IRepository<CohortCourseUser> cohortCourseUserService,
                    IMapper mapper,
                    int userId,
                    int cohortId,
                    int courseId)
        {
            // 1. Get the user
            var user = userService.GetById(userId);
            if (user == null) return TypedResults.BadRequest(new ResponseDTO<object>
            {
                Message = $"User with Id {userId} not found."
            });

            // 2. Get the cohort including its users and courses for verification steps
            var cohort = cohortService.GetById(cohortId, q =>
                q.Include(c => c.CohortCourses)
                    .ThenInclude(cc => cc.Course)
                 .Include(c => c.CohortCourses)
                    .ThenInclude(cc => cc.CohortCourseUsers)
                        .ThenInclude(ccu => ccu.User));

            if (cohort == null) return TypedResults.BadRequest(new ResponseDTO<object>
            {
                Message = $"Cohort with Id {cohortId} not found."
            });

            // 3. Verify that the course exists in this cohort
            var cohortCourse = cohort.CohortCourses.FirstOrDefault(cc => cc.CourseId == courseId);
            if (cohortCourse == null)
                return TypedResults.BadRequest(new ResponseDTO<object>
                {
                    Message = "The specified course is not part of this cohort."
                });

            // 4. Check if the user is already in this cohort
            if (cohortCourse.CohortCourseUsers.Any(cu => cu.UserId == userId))
                return TypedResults.BadRequest(new ResponseDTO<object>
                {
                    Message = "User is already a member of this cohort."
                });

            // 7. Add user to CohortCourseUser
            var existingCcu = cohortCourseUserService
                .GetAllFiltered(ccu => ccu.UserId == userId && ccu.CohortId == cohortId && ccu.CourseId == courseId)
                .FirstOrDefault();

            if (existingCcu == null)
            {
                var ccu = new CohortCourseUser
                {
                    UserId = userId,
                    User = user,
                    CohortId = cohortId,
                    Cohort = cohort,
                    CourseId = courseId,
                    Course = cohortCourse.Course
                };
                cohortCourseUserService.Insert(ccu);

                // 8. Save changes
                cohortService.Save();
                //userCourseRepo.Save();
                cohortCourseUserService.Save();

                // 9. Map response
                var ccuDTO = mapper.Map<CohortCourseUserDTO>(ccu);
                ResponseDTO<CohortCourseUserDTO> response = new ResponseDTO<CohortCourseUserDTO>()
                {
                    Message = "Success",
                    Data = ccuDTO
                };

                return TypedResults.Ok(response);
            }
            return TypedResults.BadRequest(new ResponseDTO<object>
            {
                Message = "Failed to add user."
            });
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public static async Task<IResult> DeleteUserFromCohort(
                IRepository<Cohort> cohortService,
                IRepository<User> userService,
                IRepository<CohortCourse> cohortCourseService,
                IRepository<CohortCourseUser> cohortCourseUserService,
                IMapper mapper,
                int userId,
                int cohortId,
                int courseId)
        {
            // 1. Get the user
            var user = userService.GetById(userId);
            if (user == null) return TypedResults.BadRequest(new ResponseDTO<object>
            {
                Message = $"User with Id {userId} not found."
            });

            // 2. Get the cohort including its users and courses for verification steps
            var cohort = cohortService.GetById(cohortId, q =>
                q.Include(c => c.CohortCourses)
                    .ThenInclude(cc => cc.Course)
                    .Include(c => c.CohortCourses)
                    .ThenInclude(cc => cc.CohortCourseUsers)
                        .ThenInclude(ccu => ccu.User));

            if (cohort == null) return TypedResults.BadRequest(new ResponseDTO<object>
            {
                Message = $"Cohort with Id {cohortId} not found."
            });

            // 3. Verify that the course exists in this cohort
            var cohortCourse = cohort.CohortCourses.FirstOrDefault(cc => cc.CourseId == courseId);
            if (cohortCourse == null)
                return TypedResults.BadRequest(new ResponseDTO<object>
                {
                    Message = "The specified course is not part of this cohort."
                });

            // 4. Check if the user is already in this cohort
            if (!cohortCourse.CohortCourseUsers.Any(cu => cu.UserId == userId))
                return TypedResults.BadRequest(new ResponseDTO<object>
                {
                    Message = "User is not a member of this cohort."
                });

            // 7. Add user to CohortCourseUser
            var existingCcu = cohortCourseUserService
                .GetAllFiltered(ccu => ccu.UserId == userId && ccu.CohortId == cohortId && ccu.CourseId == courseId)
                .FirstOrDefault();

            if (existingCcu != null)
            {
                //var ccu = new CohortCourseUser
                //{
                //    UserId = userId,
                //    User = user,
                //    CohortId = cohortId,
                //    Cohort = cohort,
                //    CourseId = courseId,
                //    Course = cohortCourse.Course
                //};
                cohortCourseUserService.Delete(existingCcu.CohortId, existingCcu.CourseId, existingCcu.UserId);

                // 8. Save changes
                cohortService.Save();
                //userCourseRepo.Save();
                cohortCourseUserService.Save();

                // 9. Map response
                var ccuDTO = mapper.Map<CohortCourseUserDTO>(existingCcu);
                ResponseDTO<CohortCourseUserDTO> response = new ResponseDTO<CohortCourseUserDTO>()
                {
                    Message = "Success",
                    Data = ccuDTO
                };

                return TypedResults.Ok(response);
            }
            return TypedResults.BadRequest(new ResponseDTO<object>
            {
                Message = "Failed to delete user."
            });

        }
    }
}
