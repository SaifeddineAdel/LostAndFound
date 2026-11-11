using LostAndFound.Domain.Enums;

namespace LostAndFound.Application.Features.Reports.DTOs;

public class ReportResponse
{
    public int Id { get; set; }

    public ReportType Type { get; set; }

    public ReportStatus Status { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public int SubCategoryId { get; set; }

    public int LocationId { get; set; }

    public DateTimeOffset ItemDate { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public List<string> ImageUrls { get; set; } = [];
}