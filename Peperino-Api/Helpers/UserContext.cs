using FirebaseAdmin.Auth;
using Peperino_Api.Models.User;

namespace Peperino_Api.Helpers
{
    public class UserContext
    {
        public User? PeperinoUser { get; set; }
        public UserRecord? FirebaseUser { get; set; }
    }
}
