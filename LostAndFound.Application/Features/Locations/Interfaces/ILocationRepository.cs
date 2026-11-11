using LostAndFound.Domain.Entities;

namespace LostAndFound.Application.Features.Locations.Interfaces;

public interface ILocationRepository
{
    Task<Location?> GetByIdAsync(int id);

    Task<Location?> GetByIdAdminAsync(int id);

    Task<IEnumerable<Location>> GetAllAsync();

    Task<IEnumerable<Location>> GetAllAdminAsync();

    Task<bool> ExistsByNameAsync(string name);

    Task AddAsync(Location location);

    void Update(Location location);

    Task SaveChangesAsync();
}