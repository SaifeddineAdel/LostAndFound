using LostAndFound.Application.Features.Categories.DTOs;
using LostAndFound.Domain.Entities;

namespace LostAndFound.Application.Features.Categories.Mappers;

public static class SubCategoryMapper
{
    public static SubCategory ToEntity(
        int categoryId,
        CreateSubCategoryRequest request)
    {
        return SubCategory.Create(
            categoryId,
            request.Name);
    }

    public static SubCategoryResponse ToResponse(
        SubCategory subCategory)
    {
        return new SubCategoryResponse
        {
            Id = subCategory.Id,
            Name = subCategory.Name,
            CategoryId = subCategory.CategoryId
        };
    }

    public static AdminSubCategoryResponse ToAdminResponse(
        SubCategory subCategory)
    {
        return new AdminSubCategoryResponse
        {
            Id = subCategory.Id,
            Name = subCategory.Name,
            CategoryId = subCategory.CategoryId,
            IsDeleted = subCategory.IsDeleted,
            CreatedAt = subCategory.CreatedAt,
            UpdatedAt = subCategory.UpdatedAt
        };
    }

    public static IEnumerable<SubCategoryResponse> ToResponse(
        IEnumerable<SubCategory> subCategories)
    {
        return subCategories.Select(ToResponse);
    }

    public static IEnumerable<AdminSubCategoryResponse> ToAdminResponse(
        IEnumerable<SubCategory> subCategories)
    {
        return subCategories.Select(ToAdminResponse);
    }
}