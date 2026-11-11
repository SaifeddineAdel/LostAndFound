using LostAndFound.Application.Features.Categories.DTOs;

namespace LostAndFound.Application.Features.Categories.Interfaces;

public interface ISubCategoryService
{
    Task<SubCategoryResponse> CreateAsync(
        int categoryId,
        CreateSubCategoryRequest request);

    Task<SubCategoryResponse> GetByIdAsync(
        int categoryId,
        int subCategoryId);

    Task<IEnumerable<SubCategoryResponse>> GetAllAsync(
        int categoryId);

    Task<AdminSubCategoryResponse> GetByIdAdminAsync(
        int categoryId,
        int subCategoryId);

    Task<IEnumerable<AdminSubCategoryResponse>> GetAllAdminAsync(
        int categoryId);

    Task<SubCategoryResponse> UpdateAsync(
        int categoryId,
        int subCategoryId,
        UpdateSubCategoryRequest request);

    Task DeleteAsync(
        int categoryId,
        int subCategoryId);

    Task RestoreAsync(
        int categoryId,
        int subCategoryId);
}