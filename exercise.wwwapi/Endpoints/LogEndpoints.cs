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

        [ProducesResponseType(StatusCodes.Status200OK)]
        public static async Task<IResult> CreateDeliveryLog()
        {
            return TypedResults.Ok();
        }
    }
}
