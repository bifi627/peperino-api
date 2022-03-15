using Peperino_Api.Models.User;

namespace Peperino_Api.Models.User
{
    public static partial class UserMapper
    {
        public static User AdaptToUser(this UserDto p1)
        {
            return p1 == null ? null : new User()
            {
                Username = p1.Username,
                ExternalId = p1.ExternalId
            };
        }
        public static User AdaptTo(this UserDto p2, User p3)
        {
            if (p2 == null)
            {
                return null;
            }
            User result = p3 ?? new User();
            
            result.Username = p2.Username;
            result.ExternalId = p2.ExternalId;
            return result;
            
        }
        public static UserDto AdaptToDto(this User p4)
        {
            return p4 == null ? null : new UserDto()
            {
                Username = p4.Username,
                ExternalId = p4.ExternalId
            };
        }
        public static UserDto AdaptTo(this User p5, UserDto p6)
        {
            if (p5 == null)
            {
                return null;
            }
            UserDto result = p6 ?? new UserDto();
            
            result.Username = p5.Username;
            result.ExternalId = p5.ExternalId;
            return result;
            
        }
    }
}