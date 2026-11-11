using LostAndFound.Application.Common.Exceptions;
using LostAndFound.Application.Features.Auth.Register.DTOs;
using LostAndFound.Application.Features.Auth.Register.Interfaces;
using LostAndFound.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LostAndFound.Infrastructure.Features.Auth.Register;

public sealed class RegisterService : IRegisterService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RegisterService(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<RegisterResponse> RegisterAsync(
        RegisterRequest request)
    {
        var emailExists =
            await _userManager.FindByEmailAsync(
                request.Email);

        if (emailExists is not null)
        {
            throw new ConflictException(
                "Email is already registered.");
        }

        var phoneExists =
            await _userManager.Users
                .AnyAsync(x =>
                    x.PhoneNumber == request.PhoneNumber);

        if (phoneExists)
        {
            throw new ConflictException(
                "Phone number is already registered.");
        }

        var user = request.ToEntity();

        var result =
            await _userManager.CreateAsync(
                user,
                request.Password);

        if (!result.Succeeded)
        {
            var errors =
                string.Join(
                    ", ",
                    result.Errors.Select(
                        x => x.Description));

            throw new BadRequestException(errors);
        }

        return user.ToResponse();
    }
}