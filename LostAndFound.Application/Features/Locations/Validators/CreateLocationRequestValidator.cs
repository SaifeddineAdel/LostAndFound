using FluentValidation;
using LostAndFound.Application.Features.Locations.DTOs;

namespace LostAndFound.Application.Features.Locations.Validators
{
    public class CreateLocationRequestValidator
        : AbstractValidator<CreateLocationRequest>
    {
        public CreateLocationRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")

                .MinimumLength(2)
                .WithMessage("Location name must be at least 2 characters.")

                .MaximumLength(100)
                .WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required.")

                .MinimumLength(10)
                .WithMessage("Location description must be at least 10 characters.")

                .MaximumLength(500)
                .WithMessage("Description must not exceed 500 characters.");
        }
    }
}
