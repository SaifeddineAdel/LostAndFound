using LostAndFound.Application.Features.Auth.Login.DTOs;
using LostAndFound.Infrastructure.Identity;

namespace LostAndFound.Infrastructure.Features.Auth.Login;

public static class LoginMapper
{
    public static LoginResponse ToResponse(
        this ApplicationUser user,
        string accessToken,
        DateTimeOffset expiresAt)
    {
        return new LoginResponse
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email!,
            AccessToken = accessToken,
            ExpiresAt = expiresAt
        };
    }
}