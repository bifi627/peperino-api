using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Peperino_Api.Models.User;

namespace Peperino_Api.Helpers
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class PeperinoAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string? routeKeyUserId;

        /// <param name="id">Only the user with this id has access to this endpoint.</param>
        public PeperinoAuthorizeAttribute(string? userIdRouteKey = null)
        {
            this.routeKeyUserId = userIdRouteKey;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Items.TryGetValue(AuthMiddleware.USER_CONTEXT, out var item))
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }

            if (item is not UserContext user || user.PeperinoUser is not null && !CheckUser(user.PeperinoUser, context))
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
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
    }
}
