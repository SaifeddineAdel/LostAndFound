using LostAndFound.Application.Common.Models;
using LostAndFound.Application.Features.Reports.DTOs;
using LostAndFound.Domain.Entities;

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

    Task AddAsync(Report report);

    Task SaveChangesAsync();
}