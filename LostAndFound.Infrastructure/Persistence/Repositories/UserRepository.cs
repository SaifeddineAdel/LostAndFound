using LostAndFound.Application.Features.Auth.Interfaces;
using LostAndFound.Domain.Entities;
using LostAndFound.Domain.Enums;
using LostAndFound.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace LostAndFound.Infrastructure.Features.Auth.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users
            .AnyAsync(x => x.Email == email);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    public async Task<List<User>> GetByRoleAsync(UserRole role)
    {
        return await _context.Users
            .Where(x => x.Role == role && x.IsActive)
            .ToListAsync();
    }
}