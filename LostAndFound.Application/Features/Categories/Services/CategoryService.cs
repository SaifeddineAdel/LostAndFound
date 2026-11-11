using LostAndFound.Application.Common.Exceptions;
using LostAndFound.Application.Features.Categories.DTOs;
using LostAndFound.Application.Features.Categories.Interfaces;
using LostAndFound.Application.Features.Categories.Mappers;
using LostAndFound.Domain.Entities;

namespace LostAndFound.Application.Features.Categories.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryResponse> CreateAsync(
        CreateCategoryRequest request)
    {
        var name = request.Name.Trim();

        await EnsureNameIsUniqueAsync(name);

        var category = CategoryMapper.ToEntity(request);

        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();

        return CategoryMapper.ToResponse(category);
    }

    public async Task<CategoryResponse> GetByIdAsync(int id)
    {
        var category =
            await _categoryRepository.GetByIdAsync(id);

        if (category is null)
        {
            throw new NotFoundException(
                "Category was not found.");
        }

        return CategoryMapper.ToResponse(category);
    }

    public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
    {
        var categories =
            await _categoryRepository.GetAllAsync();

        return CategoryMapper.ToResponse(categories);
    }

    public async Task<AdminCategoryResponse> GetByIdAdminAsync(int id)
    {
        var category =
            await GetCategoryOrThrowAsync(id);

        return CategoryMapper.ToAdminResponse(category);
    }

    public async Task<IEnumerable<AdminCategoryResponse>> GetAllAdminAsync()
    {
        var categories =
            await _categoryRepository.GetAllAdminAsync();

        return CategoryMapper.ToAdminResponse(categories);
    }

    public async Task<CategoryResponse> UpdateAsync(
        int id,
        UpdateCategoryRequest request)
    {
        var category =
            await GetCategoryOrThrowAsync(id);

        var newName = request.Name.Trim();

        if (!string.Equals(
            category.Name,
            newName,
            StringComparison.OrdinalIgnoreCase))
        {
            await EnsureNameIsUniqueAsync(newName);
        }

        category.UpdateDetails(
            newName,
            request.Description,
            request.RestoreAfterUpdate);

        await _categoryRepository.SaveChangesAsync();

        return CategoryMapper.ToResponse(category);
    }

    public async Task DeleteAsync(int id)
    {
        var category =
            await GetCategoryOrThrowAsync(id);

        if (category.IsDeleted)
        {
            throw new ConflictException(
                "Category is already deleted.");
        }

        category.Delete();

        await _categoryRepository.SaveChangesAsync();
    }

    public async Task RestoreAsync(int id)
    {
        var category =
            await GetCategoryOrThrowAsync(id);

        if (!category.IsDeleted)
        {
            throw new ConflictException(
                "Category is already active.");
        }

        category.Restore();

        await _categoryRepository.SaveChangesAsync();
    }

    private async Task<Category> GetCategoryOrThrowAsync(int id)
    {
        var category =
            await _categoryRepository.GetByIdAdminAsync(id);

        if (category is null)
        {
            throw new NotFoundException(
                "Category was not found.");
        }

        return category;
    }

    private async Task EnsureNameIsUniqueAsync(string name)
    {
        var exists =
            await _categoryRepository.ExistsByNameAsync(name);

        if (exists)
        {
            throw new ConflictException(
                "A category with this name already exists.");
        }
    }
}