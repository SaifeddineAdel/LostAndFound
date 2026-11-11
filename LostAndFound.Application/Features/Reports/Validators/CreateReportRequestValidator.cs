using FluentValidation;
using LostAndFound.Application.Features.Reports.DTOs;
using LostAndFound.Domain.Enums;

namespace LostAndFound.Application.Features.Reports.Validators;

public class CreateReportRequestValidator
    : AbstractValidator<CreateReportRequest>
{
    public CreateReportRequestValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid report type.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MinimumLength(2)
            .WithMessage("Title must be at least 2 characters.")
            .MaximumLength(150)
            .WithMessage("Title must not exceed 150 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MinimumLength(10)
            .WithMessage("Description must be at least 10 characters.")
            .MaximumLength(1000)
            .WithMessage("Description must not exceed 1000 characters.");

        RuleFor(x => x.PrivateDescription)
            .MaximumLength(1000)
            .When(x => x.PrivateDescription is not null)
            .WithMessage("Private description must not exceed 1000 characters.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("Category is required.");

        RuleFor(x => x.SubCategoryId)
            .GreaterThan(0)
            .WithMessage("SubCategory is required.");

        RuleFor(x => x.LocationId)
            .GreaterThan(0)
            .WithMessage("Location is required.");

        RuleFor(x => x.ItemDate)
            .NotEmpty()
            .WithMessage("Item date is required.")
            .Must(date => date <= DateTimeOffset.UtcNow)
            .WithMessage("Item date cannot be in the future.");

        RuleFor(x => x.Images)
            .NotNull()
            .WithMessage("Images list is required.")
            .Must(images => images.Count >= 1)
            .When(x =>
                x.Type == ReportType.Found &&
                x.Images is not null)
            .WithMessage(
                "At least one image is required for a found report.");
    }
}