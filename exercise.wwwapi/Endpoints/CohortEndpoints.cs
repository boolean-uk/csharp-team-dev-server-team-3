using AutoMapper;
using exercise.wwwapi.DTOs;
using exercise.wwwapi.DTOs.Cohort;
using exercise.wwwapi.DTOs.Posts;
using exercise.wwwapi.Helpers;
using exercise.wwwapi.Models;
using exercise.wwwapi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace exercise.wwwapi.Endpoints
{
    public static class CohortEndpoints
    {
        public static void ConfigureCohortEndpoints(this WebApplication app)
        {
            var cohorts = app.MapGroup("cohorts");
            cohorts.MapPost("/", CreateCohort).WithSummary("Create a cohort");
            cohorts.MapGet("/", GetAllCohorts).WithSummary("Get all cohorts");
            cohorts.MapGet("/cohortId/{cohortId}", GetCohort).WithSummary("Get a cohort by ID");
            cohorts.MapGet("/userId/{userId}", GetCohortByUserId).WithSummary("Get all cohorts a user is in by its Id");
            cohorts.MapPost("/cohortId/{cohortId}/userId/{userId}/courseId/{courseId}", AddUserToCohort).WithSummary("Add a user to a cohort");
            cohorts.MapDelete("/cohortId/{cohortId}/userId/{userId}/courseId/{courseId}", DeleteUserFromCohort).WithSummary("Delete a user from a cohort");
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public static async Task<IResult> GetCohort(IRepository<Cohort> service, IMapper mapper, int cohortId)
        {
            var result = service.GetById(cohortId, q => q
                .Include(c => c.CohortCourses)
                    .ThenInclude(cc => cc.Course)
                .Include(c => c.CohortCourses)
                    .ThenInclude(cc => cc.CohortCourseUsers)
                        .ThenInclude(ccu => ccu.User));

            if (result == null)
                return TypedResults.BadRequest(new ResponseDTO<object>
                {
                    Message = $"Cohort with id '{cohortId}' does not exists",
                    Data = result
                });

            CohortDTO cohortDTO = mapper.Map<CohortDTO>(result);
            ResponseDTO<CohortDTO> response = new ResponseDTO<CohortDTO>()
            {
                Message = "Success",
                Data = cohortDTO
            };

            return TypedResults.Ok(response);

            //return TypedResults.Ok(cohortDTOs);
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public static async Task<IResult> GetCohortByUserId(IRepository<Cohort> cohortRepo, IMapper mapper, int userId)
        {
            var results = cohortRepo.GetWithIncludes(q => q
                .Include(c => c.CohortCourses)
                    .ThenInclude(cc => cc.Course)
                .Include(c => c.CohortCourses)
                    .ThenInclude(cc => cc.CohortCourseUsers)
                        .ThenInclude(ccu => ccu.User)
            ).Where(r => r.CohortCourses.Any(cc => cc.CohortCourseUsers.Any(ccu => ccu.UserId == userId)));

            if (!results.Any())
                return TypedResults.BadRequest(new ResponseDTO<object>
                {
                    Message = $"User with id {userId} either does not exist, or is not registered in any cohorts",
                    Data = results
                });

            IEnumerable<CohortDTO> cohortDTOs = mapper.Map<IEnumerable<CohortDTO>>(results);
            ResponseDTO<IEnumerable<CohortDTO>> response = new ResponseDTO<IEnumerable<CohortDTO>>()
            {
                Message = "Success",
                Data = cohortDTOs
            };

            return TypedResults.Ok(response);
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public static async Task<IResult> GetAllCohorts(IRepository<Cohort> cohortService, IMapper mapper, ClaimsPrincipal user)
        {
            if (user.Role() != (int)Roles.teacher)
            {
                var forbiddenResponse = new ResponseDTO<object>
                {
                    Message = "You are not authorized to get all cohorts."
                };
                return TypedResults.Json(forbiddenResponse, statusCode: StatusCodes.Status403Forbidden);
            }

            var results = cohortService.GetWithIncludes(q => q
                .Include(c => c.CohortCourses)
                    .ThenInclude(cc => cc.Course)
                .Include(c => c.CohortCourses)
                    .ThenInclude(cc => cc.CohortCourseUsers)
                        .ThenInclude(ccu => ccu.User)
            );

            IEnumerable<CohortDTO> cohortDTOs = mapper.Map<IEnumerable<CohortDTO>>(results);
            ResponseDTO<IEnumerable<CohortDTO>> response = new ResponseDTO<IEnumerable<CohortDTO>>()
            {
                Message = "Success",
                Data = cohortDTOs
            };

            return TypedResults.Ok(response);
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public static async Task<IResult> CreateCohort(
            IRepository<Cohort> cohortService,
            IRepository<Course> courseService,
            ClaimsPrincipal user,
            IMapper mapper,
            CreateCohortDTO request)
        {
            if (user.Role() != (int)Roles.teacher)
            {
                var forbiddenResponse = new ResponseDTO<object>
                {
                    Message = "You are not authorized to create a new cohort."
                };
                return TypedResults.Json(forbiddenResponse, statusCode: StatusCodes.Status403Forbidden);
            }

            var results = cohortService.GetAllFiltered(c => c.Title == request.Title);
            Console.WriteLine(results);
            if (results.Any())
                return TypedResults.BadRequest(new ResponseDTO<object>
                {
                    Message = $"Cohort with name '{request.Title}' already exists",
                    Data = results
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

        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public static async Task<IResult> AddUserToCohort(
                    IRepository<Cohort> cohortService,
                    IRepository<User> userService,
                    IRepository<CohortCourse> cohortCourseService,
                    IRepository<CohortCourseUser> cohortCourseUserService,
                    IMapper mapper,
                    ClaimsPrincipal userCheck,
                    int userId,
                    int cohortId,
                    int courseId)
        {
            if (userCheck.Role() != (int)Roles.teacher)
            {
                var forbiddenResponse = new ResponseDTO<object>
                {
                    Message = "You are not authorized to add a user to a cohort."
                };
                return TypedResults.Json(forbiddenResponse, statusCode: StatusCodes.Status403Forbidden);
            }

            // 1. Get the user
            var user = userService.GetById(userId);
            if (user == null)
                return TypedResults.BadRequest(new ResponseDTO<object>
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

            if (cohort == null)
                return TypedResults.BadRequest(new ResponseDTO<object>
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
            //if (cohortCourse.CohortCourseUsers.Any(cu => cu.UserId == userId))
            //    return TypedResults.BadRequest(new ResponseDTO<object>
            //    {
            //        Message = "User is already a member of this cohort."
            //    }); 

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
                Message = "User is already in the specified course in the cohort."
            });
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public static async Task<IResult> DeleteUserFromCohort(
                IRepository<Cohort> cohortService,
                IRepository<User> userService,
                IRepository<CohortCourse> cohortCourseService,
                IRepository<CohortCourseUser> cohortCourseUserService,
                IMapper mapper,
                ClaimsPrincipal userCheck,
                int userId,
                int cohortId,
                int courseId)
        {

            if (userCheck.Role() != (int)Roles.teacher)
            {
                var forbiddenResponse = new ResponseDTO<object>
                {
                    Message = "You are not authorized to delete a user from a cohort."
                };
                return TypedResults.Json(forbiddenResponse, statusCode: StatusCodes.Status403Forbidden);
            }

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

            var isTrue = false;
            foreach (var cc in cohort.CohortCourses)
            {
                if (cc.CohortCourseUsers.Any(ccu => ccu.UserId == userId)) isTrue = true;
            }
            if (!isTrue)
            {
                return TypedResults.BadRequest(new ResponseDTO<object>
                {
                    Message = "The specified user is not part of this cohort."
                });
            }

            // 3. Verify that the course exists in this cohort
            var cohortCourse = cohort.CohortCourses.FirstOrDefault(cc => cc.CourseId == courseId);
            if (cohortCourse == null)
                return TypedResults.BadRequest(new ResponseDTO<object>
                {
                    Message = "The specified course is not part of this cohort."
                });

            // 4. Check if the user is already in this cohort
            if (!cohortCourse.CohortCourseUsers.Any(cu => cu.UserId == userId && cu.CourseId == courseId))
                return TypedResults.BadRequest(new ResponseDTO<object>
                {
                    Message = "User is in cohort, but is not taking the specified course."
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
