using Microsoft.AspNetCore.Authorization;

namespace exercise.wwwapi.Authorization.Attributes
{
    /// <summary>
    /// Requires authentication and validates that the user exists in the database
    /// </summary>
    public class AuthorizeUserExistsAttribute : AuthorizeAttribute
    {
        public AuthorizeUserExistsAttribute() : base("UserExists")
        {
        }
    }
}
