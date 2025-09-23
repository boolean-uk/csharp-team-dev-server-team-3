using exercise.wwwapi.Authorization.Handlers;
using exercise.wwwapi.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace exercise.wwwapi.Authorization.Extensions
{
    public static class AuthorizationServiceExtensions
    {
        public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Create policy that requires user to exist in database
                options.AddPolicy("UserExists", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AddRequirements(new UserExistsRequirement());
                });

                // Make this the default policy [Authorize]
                options.DefaultPolicy = options.GetPolicy("UserExists")!;
            });

            // Register the handlers
            services.AddScoped<IAuthorizationHandler, UserExistsHandler>();

            return services;
        }
    }
}
