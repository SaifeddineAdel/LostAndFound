using LostAndFound.Application.Features.Auth.Interfaces;
using LostAndFound.Domain.Entities;
using LostAndFound.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace LostAndFound.Infrastructure.Features.Auth.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByHashAsync(string tokenHash)
    {
        return await _context.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash);
    }

    public async Task AddAsync(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}