using LostAndFound.Application.Features.Auth.DTOs;

namespace LostAndFound.Application.Features.Auth.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    Task<AuthResponse> LoginAsync(LoginRequest request);

    Task<AuthResponse> RefreshAsync(RefreshTokenRequest request);

    Task LogoutAsync(string refreshToken);
}