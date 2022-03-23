using Peperino_Api.Models.User;

namespace Peperino_Api.Models.Request
{
    public class CreateUserRequest
    {
        public UserDto User { get; set; }
        public string Password { get; set; } = "";
        public string Email { get; set; } = "";

        public CreateUserRequest(UserDto user)
        {
            this.User = user;
        }
    }
}
