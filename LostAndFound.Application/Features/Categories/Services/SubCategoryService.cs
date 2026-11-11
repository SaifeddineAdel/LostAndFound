using LostAndFound.Application.Common.Exceptions;
using LostAndFound.Application.Features.Categories.DTOs;
using LostAndFound.Application.Features.Categories.Interfaces;
using LostAndFound.Application.Features.Categories.Mappers;
using LostAndFound.Domain.Entities;

namespace LostAndFound.Application.Features.Categories.Services;

public class SubCategoryService : ISubCategoryService
{
    private readonly ISubCategoryRepository _subCategoryRepository;
    private readonly ICategoryRepository _categoryRepository;

    public SubCategoryService(
        ISubCategoryRepository subCategoryRepository,
        ICategoryRepository categoryRepository)
    {
        _subCategoryRepository = subCategoryRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<SubCategoryResponse> CreateAsync(
        int categoryId,
        CreateSubCategoryRequest request)
    {
        await EnsureCategoryIsActiveAsync(categoryId);

        var name = request.Name.Trim();

        await EnsureNameIsUniqueAsync(name);

        var subCategory = SubCategoryMapper.ToEntity(
            categoryId,
            request);

        await _subCategoryRepository.AddAsync(subCategory);
        await _subCategoryRepository.SaveChangesAsync();

        return SubCategoryMapper.ToResponse(subCategory);
    }

    public async Task<SubCategoryResponse> GetByIdAsync(
        int categoryId,
        int subCategoryId)
    {
        await EnsureCategoryExistsAsync(categoryId);

        var subCategory =
            await _subCategoryRepository.GetByIdAsync(
                categoryId,
                subCategoryId);

        if (subCategory is null)
        {
            throw new NotFoundException(
                "SubCategory was not found.");
        }

        return SubCategoryMapper.ToResponse(subCategory);
    }

    public async Task<IEnumerable<SubCategoryResponse>> GetAllAsync(
        int categoryId)
    {
        await EnsureCategoryExistsAsync(categoryId);

        var subCategories =
            await _subCategoryRepository.GetAllAsync(categoryId);

        return SubCategoryMapper.ToResponse(subCategories);
    }

    public async Task<AdminSubCategoryResponse> GetByIdAdminAsync(
        int categoryId,
        int subCategoryId)
    {
        await EnsureCategoryExistsAdminAsync(categoryId);

        var subCategory =
            await GetSubCategoryOrThrowAsync(
                categoryId,
                subCategoryId);

        return SubCategoryMapper.ToAdminResponse(subCategory);
    }

    public async Task<IEnumerable<AdminSubCategoryResponse>> GetAllAdminAsync(
        int categoryId)
    {
        await EnsureCategoryExistsAdminAsync(categoryId);

        var subCategories =
            await _subCategoryRepository.GetAllAdminAsync(categoryId);

        return SubCategoryMapper.ToAdminResponse(subCategories);
    }

    public async Task<SubCategoryResponse> UpdateAsync(
        int categoryId,
        int subCategoryId,
        UpdateSubCategoryRequest request)
    {
        var subCategory =
            await GetSubCategoryOrThrowAsync(
                categoryId,
                subCategoryId);

        var newName = request.Name.Trim();

        if (!string.Equals(
            subCategory.Name,
            newName,
            StringComparison.OrdinalIgnoreCase))
        {
            await EnsureNameIsUniqueAsync(newName);
        }

        subCategory.UpdateName(
            newName,
            request.RestoreAfterUpdate);

        await _subCategoryRepository.SaveChangesAsync();

        return SubCategoryMapper.ToResponse(subCategory);
    }

    public async Task DeleteAsync(
        int categoryId,
        int subCategoryId)
    {
        var subCategory =
            await GetSubCategoryOrThrowAsync(
                categoryId,
                subCategoryId);

        if (subCategory.IsDeleted)
        {
            throw new ConflictException(
                "SubCategory is already deleted.");
        }

        subCategory.Delete();

        await _subCategoryRepository.SaveChangesAsync();
    }

    public async Task RestoreAsync(
        int categoryId,
        int subCategoryId)
    {
        var subCategory =
            await GetSubCategoryOrThrowAsync(
                categoryId,
                subCategoryId);

        if (!subCategory.IsDeleted)
        {
            throw new ConflictException(
                "SubCategory is already active.");
        }

        subCategory.Restore();

        await _subCategoryRepository.SaveChangesAsync();
    }

    private async Task<SubCategory> GetSubCategoryOrThrowAsync(
        int categoryId,
        int subCategoryId)
    {
        var subCategory =
            await _subCategoryRepository.GetByIdAdminAsync(
                categoryId,
                subCategoryId);

        if (subCategory is null)
        {
            throw new NotFoundException(
                "SubCategory was not found.");
        }

        return subCategory;
    }

    private async Task EnsureCategoryExistsAsync(int categoryId)
    {
        var category =
            await _categoryRepository.GetByIdAsync(categoryId);

        if (category is null)
        {
            throw new NotFoundException(
                "Category was not found.");
        }
    }

    private async Task EnsureCategoryExistsAdminAsync(int categoryId)
    {
        var category =
            await _categoryRepository.GetByIdAdminAsync(categoryId);

        if (category is null)
        {
            throw new NotFoundException(
                "Category was not found.");
        }
    }

    private async Task EnsureCategoryIsActiveAsync(int categoryId)
    {
        var category =
            await _categoryRepository.GetByIdAdminAsync(categoryId);

        if (category is null)
        {
            throw new NotFoundException(
                "Category was not found.");
        }

        if (category.IsDeleted)
        {
            throw new ConflictException(
                "Cannot create a subcategory under a deleted category.");
        }
    }

    private async Task EnsureNameIsUniqueAsync(string name)
    {
        var exists =
            await _subCategoryRepository.ExistsByNameAsync(name);

        if (exists)
        {
            throw new ConflictException(
                "A subcategory with this name already exists.");
        }
    }
}