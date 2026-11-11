namespace LostAndFound.Application.Features.Categories.DTOs;

public class SubCategoryResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int CategoryId { get; set; }
}