using FluentValidation;
using LostAndFound.Application.Features.Categories.DTOs;

namespace LostAndFound.Application.Features.Categories.Validators;

public class CreateSubCategoryRequestValidator
    : AbstractValidator<CreateSubCategoryRequest>
{
    public CreateSubCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MinimumLength(2)
            .WithMessage("SubCategory name must be at least 2 characters.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.");
    }
}