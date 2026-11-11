
using LostAndFound.Application.Common.Models;
using LostAndFound.Domain.Enums;

namespace LostAndFound.Application.Features.Reports.DTOs;

public class CreateReportRequest
{
    public ReportType Type { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? PrivateDescription { get; set; }

    public int CategoryId { get; set; }

    public int SubCategoryId { get; set; }

    public int LocationId { get; set; }

    public DateTimeOffset ItemDate { get; set; }

    public List<ImageUpload> Images { get; set; } = [];
}
