using LostAndFound.Application.Features.Categories.DTOs;

namespace LostAndFound.Application.Features.Categories.Interfaces;

public interface ICategoryService
{
    Task<CategoryResponse> CreateAsync(
        CreateCategoryRequest request);

    Task<CategoryResponse> GetByIdAsync(int id);

    Task<IEnumerable<CategoryResponse>> GetAllAsync();

    Task<AdminCategoryResponse> GetByIdAdminAsync(int id);

    Task<IEnumerable<AdminCategoryResponse>> GetAllAdminAsync();

    Task<CategoryResponse> UpdateAsync(
        int id,
        UpdateCategoryRequest request);

    Task DeleteAsync(int id);

    Task RestoreAsync(int id);
}