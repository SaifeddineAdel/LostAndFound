
using LostAndFound.Application.Common.Models;
using LostAndFound.Application.Features.Reports.DTOs;
using LostAndFound.Application.Features.Reports.Interfaces;
using LostAndFound.Domain.Entities;
using LostAndFound.Domain.Enums;
using LostAndFound.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace LostAndFound.Infrastructure.Persistence.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext _context;

    public ReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Report?> GetByIdAsync(int id)
    {
        return await _context.Reports
            .AsNoTracking()
            .Include(r => r.Images)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Report?> GetByIdAdminAsync(int id)
    {
        return await _context.Reports
            .Include(r => r.Images)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<PaginatedResponse<Report>> GetAllApprovedAsync(
        PaginationRequest request)
    {
        var query = _context.Reports
            .AsNoTracking()
            .Where(r => r.Status == ReportStatus.Approved)
            .OrderByDescending(r => r.CreatedAt)
            .ThenByDescending(r => r.Id);

        return await PaginateAsync(query, request);
    }

    public async Task<PaginatedResponse<Report>> SearchApprovedAsync(
        SearchReportsRequest request)
    {
        var query = _context.Reports
            .AsNoTracking()
            .Where(r => r.Status == ReportStatus.Approved);

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();

            query = query.Where(r =>
                r.Title.Contains(keyword) ||
                r.Description.Contains(keyword));
        }

        if (request.Type.HasValue)
        {
            query = query.Where(r =>
                r.Type == request.Type.Value);
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(r =>
                r.CategoryId == request.CategoryId.Value);
        }

        if (request.SubCategoryId.HasValue)
        {
            query = query.Where(r =>
                r.SubCategoryId == request.SubCategoryId.Value);
        }

        if (request.LocationId.HasValue)
        {
            query = query.Where(r =>
                r.LocationId == request.LocationId.Value);
        }

        var sortOrder = request.SortOrder?.Trim();

        query = string.Equals(
            sortOrder,
            "oldest",
            StringComparison.OrdinalIgnoreCase)
                ? query.OrderBy(r => r.CreatedAt)
                    .ThenBy(r => r.Id)
                : query.OrderByDescending(r => r.CreatedAt)
                    .ThenByDescending(r => r.Id);

        return await PaginateAsync(query, request);
    }

    public async Task<PaginatedResponse<Report>> GetMyReportsAsync(
        int userId,
        PaginationRequest request)
    {
        var query = _context.Reports
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ThenByDescending(r => r.Id);

        return await PaginateAsync(query, request);
    }

    public async Task<PaginatedResponse<Report>> GetAllPendingAsync(
        PaginationRequest request)
    {
        var query = _context.Reports
            .AsNoTracking()
            .Where(r => r.Status == ReportStatus.Pending)
            .OrderByDescending(r => r.CreatedAt)
            .ThenByDescending(r => r.Id);

        return await PaginateAsync(query, request);
    }

    private async Task<PaginatedResponse<Report>> PaginateAsync(
        IQueryable<Report> query,
        PaginationRequest request)
    {
        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Clamp(request.PageSize, 1, 50);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(r => r.Images)
            .ToListAsync();

        return new PaginatedResponse<Report>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task AddAsync(Report report)
    {
        await _context.Reports.AddAsync(report);
    }

    public void Update(Report report)
    {
        _context.Reports.Update(report);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
