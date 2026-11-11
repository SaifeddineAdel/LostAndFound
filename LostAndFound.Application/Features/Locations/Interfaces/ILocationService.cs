using LostAndFound.Application.Features.Locations.DTOs;

namespace LostAndFound.Application.Features.Locations.Interfaces;

public interface ILocationService
{
    Task<LocationResponse> CreateAsync(
        CreateLocationRequest request);

    Task<LocationResponse> GetByIdAsync(int id);

    Task<IEnumerable<LocationResponse>> GetAllAsync();

    Task<AdminLocationResponse> GetByIdAdminAsync(int id);

    Task<IEnumerable<AdminLocationResponse>> GetAllAdminAsync();

    Task<LocationResponse> UpdateAsync(
        int id,
        UpdateLocationRequest request);

    Task DeleteAsync(int id);

    Task RestoreAsync(int id);
}