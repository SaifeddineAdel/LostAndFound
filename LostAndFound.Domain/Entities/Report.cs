using LostAndFound.Domain.Enums;

namespace LostAndFound.Domain.Entities;

public class Report
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public ReportType Type { get; set; }

    public ReportStatus Status { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? PrivateDescription { get; set; }

    public int CategoryId { get; set; }

    public int SubCategoryId { get; set; }

    public int LocationId { get; set; }

    public DateTimeOffset ItemDate { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public DateTimeOffset? ApprovedAt { get; set; }

    public int? ApprovedByUserId { get; set; }

    public DateTimeOffset? ResolvedAt { get; set; }

    public int? ResolvedByUserId { get; set; }

    public Category Category { get; set; } = null!;

    public SubCategory SubCategory { get; set; } = null!;

    public Location Location { get; set; } = null!;

    public ICollection<ReportImage> Images { get; set; } = [];
}