using FluentValidation;
using LostAndFound.Application.Features.Locations.DTOs;
using LostAndFound.Application.Features.Locations.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LostAndFound.Api.Controllers;

[ApiController]
[Route("api/locations")]
[Authorize]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;
    private readonly IValidator<CreateLocationRequest> _createValidator;
    private readonly IValidator<UpdateLocationRequest> _updateValidator;

    public LocationsController(
        ILocationService locationService,
        IValidator<CreateLocationRequest> createValidator,
        IValidator<UpdateLocationRequest> updateValidator)
    {
        _locationService = locationService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LocationResponse>>> GetAll()
    {
        var locations = await _locationService.GetAllAsync();

        return Ok(locations);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LocationResponse>> GetById(int id)
    {
        var location = await _locationService.GetByIdAsync(id);

        return Ok(location);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public async Task<ActionResult<IEnumerable<AdminLocationResponse>>> GetAllAdmin()
    {
        var locations = await _locationService.GetAllAdminAsync();

        return Ok(locations);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin/{id:int}")]
    public async Task<ActionResult<AdminLocationResponse>> GetByIdAdmin(int id)
    {
        var location = await _locationService.GetByIdAdminAsync(id);

        return Ok(location);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<LocationResponse>> Create(
        CreateLocationRequest request)
    {
        var validationResult =
            await _createValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var location = await _locationService.CreateAsync(request);

        return Ok(location);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<LocationResponse>> Update(
        int id,
        UpdateLocationRequest request)
    {
        var validationResult =
            await _updateValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var location = await _locationService.UpdateAsync(id, request);

        return Ok(location);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _locationService.DeleteAsync(id);

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}/restore")]
    public async Task<IActionResult> Restore(int id)
    {
        await _locationService.RestoreAsync(id);

        return NoContent();
    }
}