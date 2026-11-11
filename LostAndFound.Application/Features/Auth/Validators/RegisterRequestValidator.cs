using FluentValidation;
using LostAndFound.Application.Features.Auth.DTOs;

namespace LostAndFound.Application.Features.Auth.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
                .WithMessage("Full name is required.")
            .MaximumLength(100)
                .WithMessage("Full name must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
                .WithMessage("Email is required.")
            .EmailAddress()
                .WithMessage("Please enter a valid email address.")
            .MaximumLength(256)
                .WithMessage("Email must not exceed 256 characters.");

        RuleFor(x => x.Password)
            .NotEmpty()
                .WithMessage("Password is required.")
            .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters long.")
            .MaximumLength(100)
                .WithMessage("Password must not exceed 100 characters.");
    }
}