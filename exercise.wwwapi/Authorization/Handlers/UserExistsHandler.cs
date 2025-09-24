using exercise.wwwapi.Authorization.Requirements;
using exercise.wwwapi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace exercise.wwwapi.Authorization.Handlers
{
    public class UserExistsHandler : AuthorizationHandler<UserExistsRequirement>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UserExistsHandler> _logger;

        public UserExistsHandler(IServiceProvider serviceProvider, ILogger<UserExistsHandler> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            UserExistsRequirement requirement)
        {
            //_logger.LogWarning("Starting user existence validation for authorization");

            //var claims = context.User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
            //_logger.LogWarning("Available claims in token: {Claims}", string.Join(", ", claims));

            // Get user ID from claims
            var userIdClaim = context.User.FindFirst(ClaimTypes.Sid)
                             ?? context.User.FindFirst(ClaimTypes.NameIdentifier);


            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                //_logger.LogWarning("No valid user ID found in token claims");
                context.Fail();
                return;
            }
            //_logger.LogWarning("Found user ID claim: {ClaimType} = {ClaimValue}", userIdClaim.Type, userIdClaim.Value);

            try
            {
                //_logger.LogWarning("Checking if user {UserId} exists in database...", userId);
                // Check if user exists in database
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                var userExists = await dbContext.Users.AnyAsync(u => u.Id == userId);

                if (userExists) context.Succeed(requirement);
                else
                {
                    _logger.LogWarning("User {UserId} not found in database", userId);
                    context.Fail();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user {UserId} exists", userId);
                context.Fail();
            }
            //_logger.LogWarning("Completed user existence validation for user {UserId}", userId);
        }
    }
}
