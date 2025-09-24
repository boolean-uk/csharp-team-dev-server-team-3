using exercise.wwwapi.Models;
using System.Security.Claims;

namespace exercise.wwwapi.Helpers
{

    public static class ClaimsPrincipalHelper
    {

        public static int? UserRealId(this ClaimsPrincipal user)
        {
            Claim? claim = user.FindFirst(ClaimTypes.Sid);

            // If the claim is null or its value is not a valid integer, return null.
            if (claim == null || !int.TryParse(claim.Value, out int userId))
            {
                return null;
            }

            return userId;
        }
        public static string UserId(this ClaimsPrincipal user)
        {
            IEnumerable<Claim> claims = user.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier);
            return claims.Count() >= 2 ? claims.ElementAt(1).Value : null;

        }

        public static string? Email(this ClaimsPrincipal user)
        {
            Claim? claim = user.FindFirst(ClaimTypes.Email);
            return claim?.Value;
        }
        public static int? Role(this ClaimsPrincipal user)
        {
            Claim? claim = user.FindFirst(ClaimTypes.Role);

            // Check if a claim was found and if its value can be parsed to a role
            if (claim != null && Enum.TryParse(claim.Value, out Roles role)) return (int)role;

            return null;
        }
    }
}
