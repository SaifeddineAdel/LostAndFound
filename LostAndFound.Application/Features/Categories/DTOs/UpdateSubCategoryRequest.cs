namespace LostAndFound.Application.Features.Categories.DTOs;

public class UpdateSubCategoryRequest
{
    public string Name { get; set; } = string.Empty;

    public bool RestoreAfterUpdate { get; set; }
}