using LostAndFound.Application.Features.Locations.Interfaces;
using LostAndFound.Domain.Entities;
using LostAndFound.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace LostAndFound.Infrastructure.Persistence.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly ApplicationDbContext _context;

    public LocationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Location?> GetByIdAsync(int id)
    {
        return await _context.Locations
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Location?> GetByIdAdminAsync(int id)
    {
        return await _context.Locations
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Location>> GetAllAsync()
    {
        return await _context.Locations
            .ToListAsync();
    }

    public async Task<IEnumerable<Location>> GetAllAdminAsync()
    {
        return await _context.Locations
            .IgnoreQueryFilters()
            .ToListAsync();
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.Locations
            .IgnoreQueryFilters()
            .AnyAsync(x => x.Name == name);
    }

    public async Task AddAsync(Location location)
    {
        await _context.Locations.AddAsync(location);
    }

    public void Update(Location location)
    {
        _context.Locations.Update(location);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}