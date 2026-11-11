using LostAndFound.Domain.Entities;

namespace LostAndFound.Application.Features.Auth.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByHashAsync(string tokenHash);

    Task AddAsync(RefreshToken refreshToken);

    Task SaveChangesAsync();
}