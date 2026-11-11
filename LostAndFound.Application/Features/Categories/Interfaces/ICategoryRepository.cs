using LostAndFound.Domain.Entities;

namespace LostAndFound.Application.Features.Categories.Interfaces;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(int id);

    Task<Category?> GetByIdAdminAsync(int id);

    Task<IEnumerable<Category>> GetAllAsync();

    Task<IEnumerable<Category>> GetAllAdminAsync();

    Task<bool> ExistsByNameAsync(string name);

    Task AddAsync(Category category);

    void Update(Category category);

    Task SaveChangesAsync();
}