using LostAndFound.Application.Features.Locations.DTOs;
using LostAndFound.Domain.Entities;

namespace LostAndFound.Application.Features.Locations.Mappers;

public static class LocationMapper
{
    public static Location ToEntity(CreateLocationRequest request)
    {
        return Location.Create(
            request.Name,
            request.Description);
    }

    public static LocationResponse ToResponse(Location location)
    {
        return new LocationResponse
        {
            Id = location.Id,
            Name = location.Name,
            Description = location.Description
        };
    }

    public static AdminLocationResponse ToAdminResponse(
        Location location)
    {
        return new AdminLocationResponse
        {
            Id = location.Id,
            Name = location.Name,
            Description = location.Description,
            IsDeleted = location.IsDeleted,
            CreatedAt = location.CreatedAt,
            UpdatedAt = location.UpdatedAt
        };
    }

    public static IEnumerable<LocationResponse> ToResponse(
        IEnumerable<Location> locations)
    {
        return locations.Select(ToResponse);
    }

    public static IEnumerable<AdminLocationResponse> ToAdminResponse(
        IEnumerable<Location> locations)
    {
        return locations.Select(ToAdminResponse);
    }
}