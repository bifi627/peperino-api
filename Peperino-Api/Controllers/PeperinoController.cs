using Microsoft.AspNetCore.Mvc;
using Peperino_Api.Helpers;
using Peperino_Api.Models.User;

namespace Peperino_Api.Controllers
{
    public abstract class PeperinoController : ControllerBase
    {
        public User CurrentUser
        {
            get
            {
                if (this.HttpContext.Items.TryGetValue(JwtMiddleware.USER_CONTEXT, out var userContext) && userContext is User user)
                {
                    return user;
                }

#pragma warning disable CS8603 // Mögliche Nullverweisrückgabe.
                return null;
#pragma warning restore CS8603 // Mögliche Nullverweisrückgabe. 
            }
        }
    }
}
