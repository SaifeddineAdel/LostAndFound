using LostAndFound.Domain.Entities;

namespace LostAndFound.Application.Features.Auth.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);

    string GenerateRefreshToken();

    string HashRefreshToken(string refreshToken);

    DateTime GetAccessTokenExpiration();

    DateTime GetRefreshTokenExpiration();
}