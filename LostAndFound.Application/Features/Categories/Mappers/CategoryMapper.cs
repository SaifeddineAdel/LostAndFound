using LostAndFound.Application.Features.Categories.DTOs;
using LostAndFound.Domain.Entities;

namespace LostAndFound.Application.Features.Categories.Mappers;

public static class CategoryMapper
{
    public static Category ToEntity(
        CreateCategoryRequest request)
    {
        return Category.Create(
            request.Name,
            request.Description);
    }

    public static CategoryResponse ToResponse(
        Category category)
    {
        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }

    public static AdminCategoryResponse ToAdminResponse(
        Category category)
    {
        return new AdminCategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsDeleted = category.IsDeleted,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    public static IEnumerable<CategoryResponse> ToResponse(
        IEnumerable<Category> categories)
    {
        return categories.Select(ToResponse);
    }

    public static IEnumerable<AdminCategoryResponse> ToAdminResponse(
        IEnumerable<Category> categories)
    {
        return categories.Select(ToAdminResponse);
    }
}