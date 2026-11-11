using LostAndFound.Application.Features.Auth.Login.DTOs;
using LostAndFound.Application.Features.Auth.Login.Interfaces;
using LostAndFound.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace LostAndFound.Infrastructure.Features.Auth.Login;

public sealed class LoginService : ILoginService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginService(
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResponse> LoginAsync(
        LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(
            request.Email);

        if (user is null)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException(
                "User account is inactive.");
        }

        var passwordValid =
            await _userManager.CheckPasswordAsync(
                user,
                request.Password);

        if (!passwordValid)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var (token, expiresAt) =
            await _jwtTokenService.GenerateTokenAsync(
                user.Id,
                user.Email!,
                user.FirstName,
                roles);

        return user.ToResponse(
            token,
            expiresAt);
    }
}