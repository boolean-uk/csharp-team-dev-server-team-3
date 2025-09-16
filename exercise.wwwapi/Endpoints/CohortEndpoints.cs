using AutoMapper;
using exercise.wwwapi.DTOs.Cohort;
using exercise.wwwapi.DTOs.Posts;
using exercise.wwwapi.Models;
using exercise.wwwapi.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Serialization;

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
            cohorts.MapPost("/{cohortId}/{userId}", AddUserToCohort).WithSummary("Add user to a cohort");
        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        public static async Task<IResult> GetCohort(IRepository<Cohort> service, IMapper mapper, int cohortId)
        {
            //var result = service.GetById(cohortId);
            var result = service.GetById(cohortId, q => q.Include(c => c.UserCohorts).ThenInclude(cu => cu.User));
            CohortDTO cohortDTOs = mapper.Map<CohortDTO>(result);

            return TypedResults.Ok(cohortDTOs);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        public static async Task<IResult> GetAllCohorts(IRepository<Cohort> service, IMapper mapper)
        {
            IEnumerable<Cohort> results = service.GetWithIncludes(q => q.Include(c => c.UserCohorts).ThenInclude(cu => cu.User));
            IEnumerable<CohortDTO> cohortDTOs = mapper.Map<IEnumerable<CohortDTO>>(results);

            return TypedResults.Ok(cohortDTOs);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        public static async Task<IResult> CreateCohort(IRepository<Cohort> service, IMapper mapper, CreateCohortDTO request)
        {
            Cohort cohort = new Cohort() { Title = request.Title };
            service.Insert(cohort);
            service.Save();
            CohortDTO response = mapper.Map<CohortDTO>(cohort);
            return TypedResults.Ok(response);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        public static async Task<IResult> AddUserToCohort(IRepository<Cohort> cohortservice, IRepository<User> userservice, IMapper mapper, int userId, int cohortId)
        {
            var user = userservice.GetById(userId);
            if (user == null) return Results.NotFound();

            Cohort cohort = cohortservice.GetById(cohortId);
            if (cohort == null) return Results.NotFound();

            UserCohort userCohort = new UserCohort() { CohortId = cohortId, UserId= userId, Cohort=cohort, User=user };
            cohort.UserCohorts.Add(userCohort);
            cohortservice.Save();
            UserCohortDTO response = mapper.Map<UserCohortDTO>(userCohort);
            return TypedResults.Ok(response);
        }
    }
}
