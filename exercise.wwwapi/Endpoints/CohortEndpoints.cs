using Microsoft.AspNetCore.Mvc;

namespace exercise.wwwapi.Endpoints
{
    public static class CohortEndpoints
    {
        public static void ConfigureCohortEndpoints(this WebApplication app)
        {
            var cohorts = app.MapGroup("cohorts");
            cohorts.MapPost("/", CreateCohort).WithSummary("Create a cohort");
        }
        
        [ProducesResponseType(StatusCodes.Status200OK)]
        private static async Task<IResult> CreateCohort()
        {
            return TypedResults.Ok();
        }
    }
}
