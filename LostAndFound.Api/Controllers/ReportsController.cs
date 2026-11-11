using LostAndFound.Api.Requests.Reports;
using LostAndFound.Application.Common.Models;
using LostAndFound.Application.Features.Reports.DTOs;
using LostAndFound.Application.Features.Reports.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LostAndFound.Api.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task<ActionResult<PaginatedResponse<ReportResponse>>> Search(
        [FromQuery] SearchReportsRequest request)
    {
        var reports = await _reportService.SearchApprovedAsync(request);

        return Ok(reports);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReportResponse>> GetById(int id)
    {
        var report = await _reportService.GetByIdAsync(id);

        return Ok(report);
    }

    [HttpGet("my-reports")]
    public async Task<ActionResult<PaginatedResponse<ReportResponse>>> GetMyReports(
        [FromQuery] PaginationRequest request)
    {
        var reports = await _reportService.GetMyReportsAsync(request);

        return Ok(reports);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(30 * 1024 * 1024)]
    public async Task<ActionResult<ReportResponse>> Create(
        [FromForm] CreateReportFormRequest form)
    {
        var uploads = new List<ImageUpload>();

        try
        {
            foreach (var image in form.Images)
            {
                uploads.Add(new ImageUpload(
                    image.OpenReadStream(),
                    image.Length,
                    image.ContentType));
            }

            var request = new CreateReportRequest
            {
                Type = form.Type,
                Title = form.Title,
                Description = form.Description,
                PrivateDescription = form.PrivateDescription,
                CategoryId = form.CategoryId,
                SubCategoryId = form.SubCategoryId,
                LocationId = form.LocationId,
                ItemDate = form.ItemDate,
                Images = uploads
            };

            var report = await _reportService.CreateAsync(request);

            return Ok(report);
        }
        finally
        {
            foreach (var upload in uploads)
            {
                await upload.Content.DisposeAsync();
            }
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin/pending")]
    public async Task<ActionResult<PaginatedResponse<AdminReportResponse>>> GetAllPending(
        [FromQuery] PaginationRequest request)
    {
        var reports = await _reportService.GetAllPendingAsync(request);

        return Ok(reports);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin/{id:int}")]
    public async Task<ActionResult<AdminReportResponse>> GetByIdAdmin(int id)
    {
        var report = await _reportService.GetByIdAdminAsync(id);

        return Ok(report);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        await _reportService.ApproveAsync(id);

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id)
    {
        await _reportService.RejectAsync(id);

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}/resolve")]
    public async Task<IActionResult> Resolve(int id)
    {
        await _reportService.ResolveAsync(id);

        return NoContent();
    }
}