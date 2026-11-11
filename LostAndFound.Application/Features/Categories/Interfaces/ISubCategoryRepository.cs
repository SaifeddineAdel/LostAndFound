using LostAndFound.Domain.Entities;

namespace LostAndFound.Application.Features.Categories.Interfaces;

public interface ISubCategoryRepository
{
    Task<SubCategory?> GetByIdAsync(
        int categoryId,
        int subCategoryId);

    Task<SubCategory?> GetByIdAdminAsync(
        int categoryId,
        int subCategoryId);

    Task<IEnumerable<SubCategory>> GetAllAsync(
        int categoryId);

    Task<IEnumerable<SubCategory>> GetAllAdminAsync(
        int categoryId);

    Task<bool> ExistsByNameAsync(string name);

    Task AddAsync(SubCategory subCategory);

    void Update(SubCategory subCategory);

    Task SaveChangesAsync();
}