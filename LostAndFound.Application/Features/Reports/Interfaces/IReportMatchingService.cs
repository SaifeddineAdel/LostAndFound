using LostAndFound.Application.Features.Reports.DTOs;

namespace LostAndFound.Application.Features.Reports.Interfaces
{
    public interface IReportMatchingService
    {
        Task<IReadOnlyList<ReportResponse>> GetSimilarReportsAsync(
            int reportId);
    }
}