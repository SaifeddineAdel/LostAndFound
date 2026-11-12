using LostAndFound.Application.Common.Exceptions;
using LostAndFound.Application.Common.Interfaces;
using LostAndFound.Application.Common.Models;
using LostAndFound.Application.Features.Categories.Interfaces;
using LostAndFound.Application.Features.Locations.Interfaces;
using LostAndFound.Application.Features.Reports.DTOs;
using LostAndFound.Application.Features.Reports.Interfaces;
using LostAndFound.Application.Features.Reports.Mappers;
using LostAndFound.Application.Messaging;
using LostAndFound.Application.Messaging.Events;
using LostAndFound.Domain.Entities;
using LostAndFound.Domain.Enums;
using LostAndFound.Infrastructure.Common.Exceptions;

namespace LostAndFound.Application.Features.Reports.Services;

public class ReportService : IReportService
{
    private const int MaxImages = 5;
    private const long MaxImageSize = 5 * 1024 * 1024;

    private readonly IReportRepository _reportRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ISubCategoryRepository _subCategoryRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IImageStorageService _imageStorageService;
    private readonly IEventPublisher _eventPublisher;

    public ReportService(
        IReportRepository reportRepository,
        ICategoryRepository categoryRepository,
        ISubCategoryRepository subCategoryRepository,
        ILocationRepository locationRepository,
        ICurrentUserService currentUserService,
        IImageStorageService imageStorageService,
        IEventPublisher eventPublisher)
    {
        _reportRepository = reportRepository;
        _categoryRepository = categoryRepository;
        _subCategoryRepository = subCategoryRepository;
        _locationRepository = locationRepository;
        _currentUserService = currentUserService;
        _imageStorageService = imageStorageService;
        _eventPublisher = eventPublisher;
    }

    public async Task<ReportResponse> CreateAsync(
        CreateReportRequest request)
    {
        if (request.Images.Count > MaxImages)
        {
            throw new InvalidImageException(
                $"A report can have at most {MaxImages} images.");
        }

        if (request.Images.Any(image =>
            image.Length <= 0 || image.Length > MaxImageSize))
        {
            throw new InvalidImageException(
                "Each image must be non-empty and 5 MB or smaller.");
        }

        var userId = _currentUserService.UserId;

        await EnsureCategoryExistsAsync(request.CategoryId);

        await EnsureSubCategoryExistsAsync(
            request.CategoryId,
            request.SubCategoryId);

        await EnsureLocationExistsAsync(request.LocationId);

        var report = ReportMapper.ToEntity(request, userId);
        var savedImageUrls = new List<string>();

        try
        {
            foreach (var image in request.Images)
            {
                var imageUrl =
                    await _imageStorageService.SaveAsync(image);

                savedImageUrls.Add(imageUrl);

                report.AddImage(ReportImage.Create(imageUrl));
            }

            await _reportRepository.AddAsync(report);
            await _reportRepository.SaveChangesAsync();
        }
        catch
        {
            foreach (var imageUrl in savedImageUrls)
            {
                try
                {
                    await _imageStorageService.DeleteAsync(imageUrl);
                }
                catch
                {
                    // Ignore cleanup errors to preserve the original exception.
                }
            }

            throw;
        }

        await _eventPublisher.PublishAsync(
            "report.created",
            new ReportCreatedEvent(
                report.Id,
                userId,
                report.Title,
                DateTimeOffset.UtcNow));

        return ReportMapper.ToResponse(report);
    }

    public async Task<ReportResponse> GetByIdAsync(int id)
    {
        var report = await _reportRepository.GetByIdAsync(id);

        if (report is null ||
            report.Status != ReportStatus.Approved)
        {
            throw new NotFoundException(
                "Report was not found.");
        }

        return ReportMapper.ToResponse(report);
    }

    public async Task<PaginatedResponse<ReportResponse>> GetAllApprovedAsync(
        PaginationRequest request)
    {
        var result =
            await _reportRepository.GetAllApprovedAsync(request);

        var items = ReportMapper
            .ToResponse(result.Items)
            .ToList();

        return new PaginatedResponse<ReportResponse>(
            items,
            result.PageNumber,
            result.PageSize,
            result.TotalCount);
    }

    public async Task<PaginatedResponse<ReportResponse>> SearchApprovedAsync(
        SearchReportsRequest request)
    {
        var result =
            await _reportRepository.SearchApprovedAsync(request);

        var items = ReportMapper
            .ToResponse(result.Items)
            .ToList();

        return new PaginatedResponse<ReportResponse>(
            items,
            result.PageNumber,
            result.PageSize,
            result.TotalCount);
    }

    public async Task<PaginatedResponse<AdminReportResponse>> GetAllPendingAsync(
        PaginationRequest request)
    {
        var result =
            await _reportRepository.GetAllPendingAsync(request);

        var items = ReportMapper
            .ToAdminResponse(result.Items)
            .ToList();

        return new PaginatedResponse<AdminReportResponse>(
            items,
            result.PageNumber,
            result.PageSize,
            result.TotalCount);
    }

    public async Task<AdminReportResponse> GetByIdAdminAsync(int id)
    {
        var report = await GetReportOrThrowAsync(id);

        return ReportMapper.ToAdminResponse(report);
    }

    public async Task<PaginatedResponse<ReportResponse>> GetMyReportsAsync(
        PaginationRequest request)
    {
        var userId = _currentUserService.UserId;

        var result =
            await _reportRepository.GetMyReportsAsync(userId, request);

        var items = ReportMapper
            .ToResponse(result.Items)
            .ToList();

        return new PaginatedResponse<ReportResponse>(
            items,
            result.PageNumber,
            result.PageSize,
            result.TotalCount);
    }

    public async Task ApproveAsync(int id)
    {
        var report = await GetReportOrThrowAsync(id);

        EnsurePending(report);

        report.Approve(_currentUserService.UserId);

        await _reportRepository.SaveChangesAsync();

        await _eventPublisher.PublishAsync(
            "report.approved",
            new ReportApprovedEvent(
                report.Id,
                report.UserId,
                DateTimeOffset.UtcNow));
    }

    public async Task RejectAsync(int id)
    {
        var report = await GetReportOrThrowAsync(id);

        EnsurePending(report);

        report.Reject();

        await _reportRepository.SaveChangesAsync();

        await _eventPublisher.PublishAsync(
            "report.rejected",
            new ReportRejectedEvent(
                report.Id,
                report.UserId,
                "Your report was rejected.",
                DateTimeOffset.UtcNow));
    }

    public async Task ResolveAsync(int id)
    {
        var report = await GetReportOrThrowAsync(id);

        EnsureApproved(report);

        report.Resolve(_currentUserService.UserId);

        await _reportRepository.SaveChangesAsync();
    }

    private async Task<Report> GetReportOrThrowAsync(int id)
    {
        var report =
            await _reportRepository.GetByIdAdminAsync(id);

        if (report is null)
        {
            throw new NotFoundException(
                "Report was not found.");
        }

        return report;
    }

    private async Task EnsureCategoryExistsAsync(int categoryId)
    {
        var category =
            await _categoryRepository.GetByIdAsync(categoryId);

        if (category is null)
        {
            throw new NotFoundException(
                "Category was not found.");
        }
    }

    private async Task EnsureSubCategoryExistsAsync(
        int categoryId,
        int subCategoryId)
    {
        var subCategory =
            await _subCategoryRepository.GetByIdAsync(
                categoryId,
                subCategoryId);

        if (subCategory is null)
        {
            throw new NotFoundException(
                "SubCategory was not found or does not belong to the selected category.");
        }
    }

    private async Task EnsureLocationExistsAsync(int locationId)
    {
        var location =
            await _locationRepository.GetByIdAsync(locationId);

        if (location is null)
        {
            throw new NotFoundException(
                "Location was not found.");
        }
    }

    private static void EnsurePending(Report report)
    {
        if (report.Status != ReportStatus.Pending)
        {
            throw new ConflictException(
                "Only pending reports can be processed.");
        }
    }

    private static void EnsureApproved(Report report)
    {
        if (report.Status != ReportStatus.Approved)
        {
            throw new ConflictException(
                "Only approved reports can be resolved.");
        }
    }
}