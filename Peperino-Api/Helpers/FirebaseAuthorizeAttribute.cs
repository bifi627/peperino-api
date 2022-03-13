using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Peperino_Api.Models;

namespace Peperino_Api.Helpers
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class FirebaseAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string? routeKeyUserId;

        /// <param name="id">Only the user with this id has access to this endpoint.</param>
        public FirebaseAuthorizeAttribute(string? routeKeyUserId = null)
        {
            this.routeKeyUserId = routeKeyUserId;
        }

        private bool CheckUser(User user, AuthorizationFilterContext context)
        {
            if (this.routeKeyUserId is not null && context.RouteData.Values.TryGetValue(this.routeKeyUserId, out var id))
            {
                if (id is string userId && user.Id.ToString() != userId)
                {
                    return false;
                }
            }

            return true;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Items.TryGetValue(JwtMiddleware.USER_CONTEXT, out var u))
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }

            if (u is not User user || !CheckUser(user, context))
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}
