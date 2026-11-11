using LostAndFound.Application.Features.Categories.Interfaces;
using LostAndFound.Domain.Entities;
using LostAndFound.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace LostAndFound.Infrastructure.Persistence.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Category?> GetByIdAdminAsync(int id)
    {
        return await _context.Categories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetAllAdminAsync()
    {
        return await _context.Categories
            .IgnoreQueryFilters()
            .ToListAsync();
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.Categories
            .IgnoreQueryFilters()
            .AnyAsync(x => x.Name == name);
    }

    public async Task AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
    }

    public void Update(Category category)
    {
        _context.Categories.Update(category);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}