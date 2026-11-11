namespace LostAndFound.Application.Features.Auth.DTOs;

public class AuthResponse
{
    public string AccessToken { get; set; } = null!;

    public string RefreshToken { get; set; } = null!;

    public DateTime AccessTokenExpiresAt { get; set; }

    public DateTime RefreshTokenExpiresAt { get; set; }

    public UserResponse User { get; set; } = null!;
}