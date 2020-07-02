using FluentValidation;

namespace Need4Protocol.Validation
{
    public class ItemValidator : AbstractValidator<Item>
    {
        public ItemValidator()
        {
            RuleFor(i => i.Name).NotEmpty().WithMessage("You must enter a name");
            RuleFor(i => i.Name).MaximumLength(5).WithMessage("Name cannot be longer than 5 characters");
        }
    }
}
