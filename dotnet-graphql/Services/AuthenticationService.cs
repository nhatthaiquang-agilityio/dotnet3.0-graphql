using System.Security.Claims;
using GraphQL.Types;

namespace dotnet_graphql.Services
{
    public static class AuthenticationService
    {
        /// <summary>
        /// Get Authentication of Context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsAuthenticated(ResolveFieldContext<object> context)
        {
            var user = (ClaimsPrincipal)context.UserContext;
            return user != null && ((ClaimsIdentity) user.Identity).IsAuthenticated;
        }
    }
}
