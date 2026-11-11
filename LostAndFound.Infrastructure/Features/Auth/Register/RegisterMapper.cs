using LostAndFound.Application.Features.Auth.Register.DTOs;
using LostAndFound.Infrastructure.Identity;

namespace LostAndFound.Infrastructure.Features.Auth.Register;

public static class RegisterMapper
{
    public static ApplicationUser ToEntity(
        this RegisterRequest request)
    {
        return new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email,
            PhoneNumber = request.PhoneNumber,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public static RegisterResponse ToResponse(
    this ApplicationUser user)
    {
        return new RegisterResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber!
        };
    }
}