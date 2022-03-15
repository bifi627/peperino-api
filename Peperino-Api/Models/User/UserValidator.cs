using FluentValidation;

namespace Peperino_Api.Models.User
{
    public class UserValidator : AbstractValidator<UserDto>
    {
        public UserValidator()
        {
            RuleFor(user => user.Username).NotEmpty();
            RuleFor(user => user.ExternalId).NotEmpty();
        }
    }
}
