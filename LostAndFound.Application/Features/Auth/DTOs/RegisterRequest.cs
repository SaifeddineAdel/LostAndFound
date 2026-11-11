namespace LostAndFound.Application.Features.Auth.DTOs;

public class RegisterRequest
{
    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;
}