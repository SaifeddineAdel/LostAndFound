
using LostAndFound.Application.Common.Models;
using LostAndFound.Application.Features.Reports.DTOs;

namespace LostAndFound.Application.Features.Reports.Interfaces;

public interface IReportService
{
    Task<ReportResponse> CreateAsync(CreateReportRequest request);

    Task<ReportResponse> GetByIdAsync(int id);

    Task<PaginatedResponse<ReportResponse>> GetAllApprovedAsync(
        PaginationRequest request);

    Task<PaginatedResponse<ReportResponse>> SearchApprovedAsync(
        SearchReportsRequest request);

    Task<PaginatedResponse<AdminReportResponse>> GetAllPendingAsync(
        PaginationRequest request);

    Task<AdminReportResponse> GetByIdAdminAsync(int id);

    Task<PaginatedResponse<ReportResponse>> GetMyReportsAsync(
        PaginationRequest request);

    Task ApproveAsync(int id);

    Task RejectAsync(int id);

    Task ResolveAsync(int id);
}
