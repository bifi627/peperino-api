using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Peperino_Api.Services;
using System.Collections.Concurrent;

namespace Peperino_Api.Helpers
{
    public class AuthMiddleware
    {
        public const string USER_CONTEXT = "USER_CONTEXT";

        private static readonly ConcurrentDictionary<string, UserContext> _userContexts = new ConcurrentDictionary<string, UserContext>();

        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, FirebaseApp firebase, IUserManagementService userService)
        {
            var token = GetToken(context);

            if (!string.IsNullOrEmpty(token))
            {
                await AttachUserToContext(context, firebase, token, userService);
            }

            await _next(context);
        }

        private static string? GetToken(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            return token;
        }

        private async static Task AttachUserToContext(HttpContext context, FirebaseApp firebase, string token, IUserManagementService userService)
        {
            var result = await FirebaseAuth.GetAuth(firebase).VerifyIdTokenAsync(token);

            if (result is not null)
            {
                // TODO: Cache token verification, remember verified tokens?

                // Get user context from cache
                //if (_userContexts.TryGetValue(token, out UserContext? userContext))
                //{
                //    if (userContext is not null)
                //    {
                //        context.Items.Add(USER_CONTEXT, userContext);
                //        return;
                //    }
                //}
                //else
                {
                    // Create new user context
                    var externalId = result.Uid;

                    var firebaseUser = await FirebaseAuth.GetAuth(firebase).GetUserAsync(externalId);
                    var user = await userService.GetByExternalId(externalId);

                    UserContext newUserContext = new() { FirebaseUser = firebaseUser, PeperinoUser = user };

                    //if (_userContexts.TryAdd(token, newUserContext))
                    {
                        context.Items.Add(USER_CONTEXT, newUserContext);
                    }
                }
            }
        }
    }
}
