using LostAndFound.Domain.Entities;
using LostAndFound.Domain.Enums;

namespace LostAndFound.Application.Features.Auth.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);

    Task<User?> GetByIdAsync(int id);

    Task<bool> EmailExistsAsync(string email);

    Task<List<User>> GetByRoleAsync(UserRole role);

    Task AddAsync(User user);

    Task SaveChangesAsync();
}