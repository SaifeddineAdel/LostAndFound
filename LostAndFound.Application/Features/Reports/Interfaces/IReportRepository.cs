using LostAndFound.Application.Common.Models;
using LostAndFound.Application.Features.Reports.DTOs;
using LostAndFound.Domain.Entities;
using LostAndFound.Domain.Enums;

namespace LostAndFound.Application.Features.Reports.Interfaces;

public interface IReportRepository
{
    Task<Report?> GetByIdAsync(int id);

    Task<Report?> GetByIdAdminAsync(int id);

    Task<PaginatedResponse<Report>> GetAllApprovedAsync(
        PaginationRequest request);

    Task<PaginatedResponse<Report>> SearchApprovedAsync(
        SearchReportsRequest request);

    Task<PaginatedResponse<Report>> GetMyReportsAsync(
        int userId,
        PaginationRequest request);

    Task<PaginatedResponse<Report>> GetAllPendingAsync(
        PaginationRequest request);

    Task<IReadOnlyList<Report>> GetApprovedOppositeReportsAsync(
        int reportId,
        ReportType oppositeType,
        int categoryId,
        int subCategoryId);

    Task AddAsync(Report report);

    Task SaveChangesAsync();
}