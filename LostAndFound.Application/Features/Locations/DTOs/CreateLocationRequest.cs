namespace LostAndFound.Application.Features.Locations.DTOs;

public class CreateLocationRequest
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}