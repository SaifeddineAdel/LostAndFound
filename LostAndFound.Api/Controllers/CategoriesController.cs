using FluentValidation;
using LostAndFound.Application.Features.Categories.DTOs;
using LostAndFound.Application.Features.Categories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LostAndFound.Api.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    private readonly IValidator<CreateCategoryRequest>
        _createValidator;

    private readonly IValidator<UpdateCategoryRequest>
        _updateValidator;

    public CategoriesController(
        ICategoryService categoryService,
        IValidator<CreateCategoryRequest> createValidator,
        IValidator<UpdateCategoryRequest> updateValidator)
    {
        _categoryService = categoryService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAll()
    {
        var categories =
            await _categoryService.GetAllAsync();

        return Ok(categories);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryResponse>> GetById(
        int id)
    {
        var category =
            await _categoryService.GetByIdAsync(id);

        return Ok(category);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public async Task<ActionResult<IEnumerable<AdminCategoryResponse>>>
        GetAllAdmin()
    {
        var categories =
            await _categoryService.GetAllAdminAsync();

        return Ok(categories);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin/{id:int}")]
    public async Task<ActionResult<AdminCategoryResponse>>
        GetByIdAdmin(int id)
    {
        var category =
            await _categoryService.GetByIdAdminAsync(id);

        return Ok(category);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> Create(
        CreateCategoryRequest request)
    {
        var validationResult =
            await _createValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var category =
            await _categoryService.CreateAsync(request);

        return Ok(category);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryResponse>> Update(
        int id,
        UpdateCategoryRequest request)
    {
        var validationResult =
            await _updateValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var category =
            await _categoryService.UpdateAsync(
                id,
                request);

        return Ok(category);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _categoryService.DeleteAsync(id);

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}/restore")]
    public async Task<IActionResult> Restore(int id)
    {
        await _categoryService.RestoreAsync(id);

        return NoContent();
    }
}