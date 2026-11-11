namespace LostAndFound.Application.Features.Auth.Login.Interfaces;

public interface IJwtTokenService
{
    Task<(string Token, DateTimeOffset ExpiresAt)> GenerateTokenAsync(
        int userId,
        string email,
        string firstName,
        IList<string> roles);
}