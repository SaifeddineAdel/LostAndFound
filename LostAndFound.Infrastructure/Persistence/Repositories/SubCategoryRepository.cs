using LostAndFound.Application.Features.Categories.Interfaces;
using LostAndFound.Domain.Entities;
using LostAndFound.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace LostAndFound.Infrastructure.Persistence.Repositories;

public class SubCategoryRepository : ISubCategoryRepository
{
    private readonly ApplicationDbContext _context;

    public SubCategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SubCategory?> GetByIdAsync(
        int categoryId,
        int subCategoryId)
    {
        return await _context.SubCategories
            .FirstOrDefaultAsync(x =>
                x.Id == subCategoryId &&
                x.CategoryId == categoryId);
    }

    public async Task<SubCategory?> GetByIdAdminAsync(
        int categoryId,
        int subCategoryId)
    {
        return await _context.SubCategories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x =>
                x.Id == subCategoryId &&
                x.CategoryId == categoryId);
    }

    public async Task<IEnumerable<SubCategory>> GetAllAsync(
        int categoryId)
    {
        return await _context.SubCategories
            .Where(x => x.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<IEnumerable<SubCategory>> GetAllAdminAsync(
        int categoryId)
    {
        return await _context.SubCategories
            .IgnoreQueryFilters()
            .Where(x => x.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.SubCategories
            .IgnoreQueryFilters()
            .AnyAsync(x => x.Name == name);
    }

    public async Task AddAsync(SubCategory subCategory)
    {
        await _context.SubCategories.AddAsync(subCategory);
    }

    public void Update(SubCategory subCategory)
    {
        _context.SubCategories.Update(subCategory);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}