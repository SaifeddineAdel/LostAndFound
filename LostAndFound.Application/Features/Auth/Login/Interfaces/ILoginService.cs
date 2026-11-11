using LostAndFound.Application.Features.Auth.Login.DTOs;

namespace LostAndFound.Application.Features.Auth.Login.Interfaces;

public interface ILoginService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}