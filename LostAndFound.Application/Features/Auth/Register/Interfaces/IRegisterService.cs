using LostAndFound.Application.Features.Auth.Register.DTOs;

namespace LostAndFound.Application.Features.Auth.Register.Interfaces;

public interface IRegisterService
{
    Task<RegisterResponse> RegisterAsync(
        RegisterRequest request);
}