using LostAndFound.Application.Common.Exceptions;
using LostAndFound.Application.Features.Reports.DTOs;
using LostAndFound.Application.Features.Reports.Interfaces;
using LostAndFound.Application.Features.Reports.Mappers;
using LostAndFound.Domain.Entities;
using LostAndFound.Domain.Enums;
using LostAndFound.Domain.Exceptions;

namespace LostAndFound.Application.Features.Reports.Services;

public class ReportMatchingService : IReportMatchingService
{
    private const int MaxResults = 10;
    private const double MinimumSimilarityScore = 30;

    private readonly IReportRepository _reportRepository;
    private readonly SimilarityCalculator _similarityCalculator;

    public ReportMatchingService(
        IReportRepository reportRepository,
        SimilarityCalculator similarityCalculator)
    {
        _reportRepository = reportRepository;
        _similarityCalculator = similarityCalculator;
    }

    public async Task<IReadOnlyList<ReportResponse>> GetSimilarReportsAsync(
        int reportId)
    {
        var originalReport = await _reportRepository.GetByIdAsync(reportId);

        if (originalReport is null)
            throw new NotFoundException("Report was not found.");

        if (originalReport.Status != ReportStatus.Approved)
            throw new ConflictException(
                "Similar reports can only be found for approved reports.");

        var oppositeType = originalReport.Type switch
        {
            ReportType.Lost => ReportType.Found,
            ReportType.Found => ReportType.Lost,
            _ => throw new DomainException("Invalid report type.")
        };

        var candidates =
            await _reportRepository.GetApprovedOppositeReportsAsync(
                originalReport.Id,
                oppositeType,
                originalReport.CategoryId,
                originalReport.SubCategoryId);

        var topMatches = new PriorityQueue<Report, double>();

        foreach (var candidate in candidates)
        {
            var score = _similarityCalculator.Calculate(
                originalReport,
                candidate);

            if (score < MinimumSimilarityScore)
                continue;

            if (topMatches.Count < MaxResults)
            {
                topMatches.Enqueue(candidate, score);
                continue;
            }

            topMatches.TryPeek(out _, out var lowestScore);

            if (score > lowestScore)
            {
                topMatches.Dequeue();
                topMatches.Enqueue(candidate, score);
            }
        }

        return topMatches.UnorderedItems
            .OrderByDescending(item => item.Priority)
            .Select(item => ReportMapper.ToResponse(item.Element))
            .ToList();
    }
}