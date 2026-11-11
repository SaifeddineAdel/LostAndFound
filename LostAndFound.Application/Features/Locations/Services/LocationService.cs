using LostAndFound.Application.Common.Exceptions;
using LostAndFound.Application.Features.Locations.DTOs;
using LostAndFound.Application.Features.Locations.Interfaces;
using LostAndFound.Application.Features.Locations.Mappers;
using LostAndFound.Domain.Entities;

namespace LostAndFound.Application.Features.Locations.Services;

public class LocationService : ILocationService
{
    private readonly ILocationRepository _locationRepository;

    public LocationService(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<LocationResponse> CreateAsync(
        CreateLocationRequest request)
    {
        var name = request.Name.Trim();

        await EnsureNameIsUniqueAsync(name);

        var location = LocationMapper.ToEntity(request);

        await _locationRepository.AddAsync(location);
        await _locationRepository.SaveChangesAsync();

        return LocationMapper.ToResponse(location);
    }

    public async Task<LocationResponse> GetByIdAsync(int id)
    {
        var location =
            await _locationRepository.GetByIdAsync(id);

        if (location is null)
        {
            throw new NotFoundException(
                "Location was not found.");
        }

        return LocationMapper.ToResponse(location);
    }

    public async Task<IEnumerable<LocationResponse>> GetAllAsync()
    {
        var locations =
            await _locationRepository.GetAllAsync();

        return LocationMapper.ToResponse(locations);
    }

    public async Task<AdminLocationResponse> GetByIdAdminAsync(int id)
    {
        var location =
            await GetLocationOrThrowAsync(id);

        return LocationMapper.ToAdminResponse(location);
    }

    public async Task<IEnumerable<AdminLocationResponse>> GetAllAdminAsync()
    {
        var locations =
            await _locationRepository.GetAllAdminAsync();

        return LocationMapper.ToAdminResponse(locations);
    }

    public async Task<LocationResponse> UpdateAsync(
        int id,
        UpdateLocationRequest request)
    {
        var location =
            await GetLocationOrThrowAsync(id);

        var newName = request.Name.Trim();

        if (!string.Equals(
            location.Name,
            newName,
            StringComparison.OrdinalIgnoreCase))
        {
            await EnsureNameIsUniqueAsync(newName);
        }

        location.UpdateDetails(
            newName,
            request.Description,
            request.RestoreAfterUpdate);

        await _locationRepository.SaveChangesAsync();

        return LocationMapper.ToResponse(location);
    }

    public async Task DeleteAsync(int id)
    {
        var location =
            await GetLocationOrThrowAsync(id);

        if (location.IsDeleted)
        {
            throw new ConflictException(
                "Location is already deleted.");
        }

        location.Delete();

        await _locationRepository.SaveChangesAsync();
    }

    public async Task RestoreAsync(int id)
    {
        var location =
            await GetLocationOrThrowAsync(id);

        if (!location.IsDeleted)
        {
            throw new ConflictException(
                "Location is already active.");
        }

        location.Restore();

        await _locationRepository.SaveChangesAsync();
    }

    private async Task<Location> GetLocationOrThrowAsync(int id)
    {
        var location =
            await _locationRepository.GetByIdAdminAsync(id);

        if (location is null)
        {
            throw new NotFoundException(
                "Location was not found.");
        }

        return location;
    }

    private async Task EnsureNameIsUniqueAsync(string name)
    {
        var exists =
            await _locationRepository.ExistsByNameAsync(name);

        if (exists)
        {
            throw new ConflictException(
                "A location with this name already exists.");
        }
    }
}