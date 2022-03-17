using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using Peperino_Api.Helpers;
using Peperino_Api.Models.User;

namespace Peperino_Api.Controllers
{
    public abstract class PeperinoController : ControllerBase
    {
        public User? CurrentUser
        {
            get
            {
                return this.UserContext?.PeperinoUser;
            }
        }

        public UserRecord? FirebaseUser
        {
            get
            {
                return this.UserContext?.FirebaseUser;
            }
        }

        public UserContext UserContext
        {
            get
            {
                if (this.HttpContext.Items.TryGetValue(JwtMiddleware.USER_CONTEXT, out var item) && item is UserContext userContext)
                {
                    return userContext;
                }

#pragma warning disable CS8603 // Mögliche Nullverweisrückgabe.
                return null;
#pragma warning restore CS8603 // Mögliche Nullverweisrückgabe. 
            }
        }
    }
}
