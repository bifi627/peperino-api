using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Peperino_Api.Services;
using System.Collections.Concurrent;

namespace Peperino_Api.Helpers
{
    public class JwtMiddleware
    {
        public const string USER_CONTEXT = "USER_CONTEXT";

        private static readonly ConcurrentDictionary<string, UserContext> _userContexts = new ConcurrentDictionary<string, UserContext>();

        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, FirebaseApp firebase, IUserService userService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                await AttachUserToContext(context, firebase, token, userService);
            }

            await _next(context);
        }

        private static async Task AttachUserToContext(HttpContext context, FirebaseApp firebase, string token, IUserService userService)
        {
            try
            {
                var result = await FirebaseAuth.GetAuth(firebase).VerifyIdTokenAsync(token);

                if (result is not null)
                {
                    // Get user context from cache
                    if (_userContexts.TryGetValue(token, out UserContext? userContext))
                    {
                        if (userContext is not null)
                        {
                            context.Items.Add(USER_CONTEXT, userContext);
                            return;
                        }
                    }
                    else
                    {
                        // Create new user context
                        var externalId = result.Uid;

                        var firebaseUser = await FirebaseAuth.GetAuth(firebase).GetUserAsync(externalId);
                        var user = await userService.GetByExternalId(externalId);

                        UserContext newUserContext = new() { FirebaseUser = firebaseUser, PeperinoUser = user };

                        if (_userContexts.TryAdd(token, newUserContext))
                        {
                            context.Items.Add(USER_CONTEXT, newUserContext);
                        }
                    }
                }
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
    }
}
