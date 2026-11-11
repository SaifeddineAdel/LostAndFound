
using LostAndFound.Application.Common.Models;
using LostAndFound.Domain.Enums;

namespace LostAndFound.Application.Features.Reports.DTOs;

public class SearchReportsRequest : PaginationRequest
{
    public string? Keyword { get; set; }

    public ReportType? Type { get; set; }

    public int? CategoryId { get; set; }

    public int? SubCategoryId { get; set; }

    public int? LocationId { get; set; }

    public string SortOrder { get; set; } = "newest";
}
