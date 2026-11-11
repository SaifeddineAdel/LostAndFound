using LostAndFound.Domain.Enums;

namespace LostAndFound.Application.Features.Auth.DTOs;

public class UserResponse
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public UserRole Role { get; set; }
}