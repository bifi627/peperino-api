using FluentValidation;

namespace Peperino_Api.Models.List
{
    public class ListValidator : AbstractValidator<ListDto>
    {
        public ListValidator()
        {
            RuleFor(list => list.Name).NotEmpty();
            RuleFor(list => list.Slug).NotNull();
            RuleFor(list => list.ListItems).NotNull();
            RuleForEach(list => list.ListItems).SetValidator(new ListItemValidator());
        }
    }

    public class ListItemValidator : AbstractValidator<ListItemDto>
    {
        public ListItemValidator()
        {
            RuleFor(item => item.Text).NotEmpty();
            RuleFor(item => item.Id).NotEmpty();
            RuleFor(item => item.Type).NotNull();
            RuleFor(item => item.Checked).NotNull();
        }
    }
}
