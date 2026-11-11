namespace LostAndFound.Application.Features.Locations.DTOs;

public class LocationResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}