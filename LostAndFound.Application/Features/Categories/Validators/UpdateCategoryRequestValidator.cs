using FluentValidation;
using LostAndFound.Application.Features.Categories.DTOs;

namespace LostAndFound.Application.Features.Categories.Validators;

public class UpdateCategoryRequestValidator
    : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MinimumLength(2)
            .WithMessage("Category name must be at least 2 characters.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MinimumLength(10)
            .WithMessage("Category description must be at least 10 characters.")
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters.");
    }
}