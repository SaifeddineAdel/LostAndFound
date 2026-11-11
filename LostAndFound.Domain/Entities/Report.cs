using LostAndFound.Domain.Enums;
using LostAndFound.Domain.Exceptions;

namespace LostAndFound.Domain.Entities;

public class Report
{
    private readonly List<ReportImage> _images = [];

    public int Id { get; private set; }

    public int UserId { get; private set; }

    public ReportType Type { get; private set; }

    public ReportStatus Status { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public string? PrivateDescription { get; private set; }

    public int CategoryId { get; private set; }

    public int SubCategoryId { get; private set; }

    public int LocationId { get; private set; }

    public DateTimeOffset ItemDate { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public DateTimeOffset? ApprovedAt { get; private set; }

    public int? ApprovedByUserId { get; private set; }

    public DateTimeOffset? ResolvedAt { get; private set; }

    public int? ResolvedByUserId { get; private set; }

    public Category Category { get; private set; } = null!;

    public SubCategory SubCategory { get; private set; } = null!;

    public Location Location { get; private set; } = null!;

    public IReadOnlyCollection<ReportImage> Images =>
        _images.AsReadOnly();

    private Report()
    {
    }

    private Report(
        int userId,
        ReportType type,
        string title,
        string description,
        string? privateDescription,
        int categoryId,
        int subCategoryId,
        int locationId,
        DateTimeOffset itemDate)
    {
        if (userId <= 0)
        {
            throw new DomainException("User ID must be greater than zero.");
        }

        if (!Enum.IsDefined(type))
        {
            throw new DomainException("Invalid report type.");
        }

        if (categoryId <= 0)
        {
            throw new DomainException("Category ID must be greater than zero.");
        }

        if (subCategoryId <= 0)
        {
            throw new DomainException("SubCategory ID must be greater than zero.");
        }

        if (locationId <= 0)
        {
            throw new DomainException("Location ID must be greater than zero.");
        }

        if (itemDate > DateTimeOffset.UtcNow)
        {
            throw new DomainException("Item date cannot be in the future.");
        }

        UserId = userId;
        Type = type;
        Status = ReportStatus.Pending;
        Title = NormalizeRequiredText(title, "Title");
        Description = NormalizeRequiredText(description, "Description");
        PrivateDescription = NormalizeOptionalText(privateDescription);
        CategoryId = categoryId;
        SubCategoryId = subCategoryId;
        LocationId = locationId;
        ItemDate = itemDate;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static Report Create(
        int userId,
        ReportType type,
        string title,
        string description,
        string? privateDescription,
        int categoryId,
        int subCategoryId,
        int locationId,
        DateTimeOffset itemDate)
    {
        return new Report(
            userId,
            type,
            title,
            description,
            privateDescription,
            categoryId,
            subCategoryId,
            locationId,
            itemDate);
    }

    public void AddImage(ReportImage image)
    {
        if (image is null)
        {
            throw new DomainException("Report image cannot be null.");
        }

        if (_images.Contains(image))
        {
            throw new DomainException("Image is already attached to this report.");
        }

        _images.Add(image);
    }

    public void Approve(int approvedByUserId)
    {
        EnsurePending();

        if (approvedByUserId <= 0)
        {
            throw new DomainException(
                "Approver user ID must be greater than zero.");
        }

        Status = ReportStatus.Approved;
        ApprovedByUserId = approvedByUserId;
        ApprovedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Reject()
    {
        EnsurePending();

        Status = ReportStatus.Rejected;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Resolve(int resolvedByUserId)
    {
        EnsureApproved();

        if (resolvedByUserId <= 0)
        {
            throw new DomainException(
                "Resolver user ID must be greater than zero.");
        }

        Status = ReportStatus.Resolved;
        ResolvedAt = DateTimeOffset.UtcNow;
        ResolvedByUserId = resolvedByUserId;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private void EnsurePending()
    {
        if (Status != ReportStatus.Pending)
        {
            throw new DomainException(
                "Only pending reports can be processed.");
        }
    }

    private void EnsureApproved()
    {
        if (Status != ReportStatus.Approved)
        {
            throw new DomainException(
                "Only approved reports can be resolved.");
        }
    }

    private static string NormalizeRequiredText(
        string value,
        string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(
                $"{propertyName} cannot be empty.");
        }

        return value.Trim();
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}