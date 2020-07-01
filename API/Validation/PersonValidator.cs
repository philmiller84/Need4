using FluentValidation;

namespace Need4Protocol.Validation
{
    public class ItemValidator : AbstractValidator<Item>
    {
        public ItemValidator()
        {
            RuleFor(p => p.Name).NotEmpty().WithMessage("You must enter a name");
            RuleFor(p => p.Name).MaximumLength(5).WithMessage("Name cannot be longer than 5 characters");
        }
    }
}
