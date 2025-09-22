using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace exercise.wwwapi.Endpoints
{
    public static class LogEndpoints
    {
        public static void ConfigureLogEndpoints(this WebApplication app)
        {
            var logs = app.MapGroup("/logs");
            logs.MapPost("/", CreateDeliveryLog).WithSummary("Create a delivery log");
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        private static Task<IResult> CreateDeliveryLog()
        {
            return Task.FromResult<IResult>(TypedResults.Ok());
        }
    }
}
