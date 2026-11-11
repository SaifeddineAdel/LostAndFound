using FluentValidation;
using LostAndFound.Application.Features.Categories.DTOs;
using LostAndFound.Application.Features.Categories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LostAndFound.Api.Controllers;

[ApiController]
[Route("api/categories/{categoryId:int}/subcategories")]
[Authorize]
public class SubCategoriesController : ControllerBase
{
    private readonly ISubCategoryService _subCategoryService;
    private readonly IValidator<CreateSubCategoryRequest> _createValidator;
    private readonly IValidator<UpdateSubCategoryRequest> _updateValidator;

    public SubCategoriesController(
        ISubCategoryService subCategoryService,
        IValidator<CreateSubCategoryRequest> createValidator,
        IValidator<UpdateSubCategoryRequest> updateValidator)
    {
        _subCategoryService = subCategoryService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubCategoryResponse>>> GetAll(
        int categoryId)
    {
        var subCategories =
            await _subCategoryService.GetAllAsync(categoryId);

        return Ok(subCategories);
    }

    [HttpGet("{subCategoryId:int}")]
    public async Task<ActionResult<SubCategoryResponse>> GetById(
        int categoryId,
        int subCategoryId)
    {
        var subCategory =
            await _subCategoryService.GetByIdAsync(
                categoryId,
                subCategoryId);

        return Ok(subCategory);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public async Task<
        ActionResult<IEnumerable<AdminSubCategoryResponse>>>
        GetAllAdmin(int categoryId)
    {
        var subCategories =
            await _subCategoryService.GetAllAdminAsync(categoryId);

        return Ok(subCategories);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin/{subCategoryId:int}")]
    public async Task<ActionResult<AdminSubCategoryResponse>>
        GetByIdAdmin(
            int categoryId,
            int subCategoryId)
    {
        var subCategory =
            await _subCategoryService.GetByIdAdminAsync(
                categoryId,
                subCategoryId);

        return Ok(subCategory);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<SubCategoryResponse>> Create(
        int categoryId,
        CreateSubCategoryRequest request)
    {
        var validationResult =
            await _createValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var subCategory =
            await _subCategoryService.CreateAsync(
                categoryId,
                request);

        return Ok(subCategory);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{subCategoryId:int}")]
    public async Task<ActionResult<SubCategoryResponse>> Update(
        int categoryId,
        int subCategoryId,
        UpdateSubCategoryRequest request)
    {
        var validationResult =
            await _updateValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var subCategory =
            await _subCategoryService.UpdateAsync(
                categoryId,
                subCategoryId,
                request);

        return Ok(subCategory);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{subCategoryId:int}")]
    public async Task<IActionResult> Delete(
        int categoryId,
        int subCategoryId)
    {
        await _subCategoryService.DeleteAsync(
            categoryId,
            subCategoryId);

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{subCategoryId:int}/restore")]
    public async Task<IActionResult> Restore(
        int categoryId,
        int subCategoryId)
    {
        await _subCategoryService.RestoreAsync(
            categoryId,
            subCategoryId);

        return NoContent();
    }
}