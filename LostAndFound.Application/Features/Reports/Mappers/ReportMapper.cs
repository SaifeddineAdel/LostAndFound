using LostAndFound.Application.Features.Reports.DTOs;
using LostAndFound.Domain.Entities;
using LostAndFound.Domain.Enums;

namespace LostAndFound.Application.Features.Reports.Mappers;

public static class ReportMapper
{
    public static Report ToEntity(
        CreateReportRequest request,
        int userId)
    {
        return new Report
        {
            UserId = userId,
            Type = request.Type,
            Status = ReportStatus.Pending,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            PrivateDescription =
                request.PrivateDescription?.Trim(),
            CategoryId = request.CategoryId,
            SubCategoryId = request.SubCategoryId,
            LocationId = request.LocationId,
            ItemDate = request.ItemDate,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public static ReportResponse ToResponse(
        Report report)
    {
        return new ReportResponse
        {
            Id = report.Id,
            Type = report.Type,
            Status = report.Status,
            Title = report.Title,
            Description = report.Description,
            CategoryId = report.CategoryId,
            SubCategoryId = report.SubCategoryId,
            LocationId = report.LocationId,
            ItemDate = report.ItemDate,
            CreatedAt = report.CreatedAt,
            ImageUrls = report.Images
                .Select(x => GetImageUrl(x.ImageUrl))
                .ToList()
        };
    }

    public static AdminReportResponse ToAdminResponse(
        Report report)
    {
        return new AdminReportResponse
        {
            Id = report.Id,
            UserId = report.UserId,
            Type = report.Type,
            Status = report.Status,
            Title = report.Title,
            Description = report.Description,
            PrivateDescription = report.PrivateDescription,
            CategoryId = report.CategoryId,
            SubCategoryId = report.SubCategoryId,
            LocationId = report.LocationId,
            ItemDate = report.ItemDate,
            CreatedAt = report.CreatedAt,
            UpdatedAt = report.UpdatedAt,
            ApprovedAt = report.ApprovedAt,
            ApprovedByUserId = report.ApprovedByUserId,
            ResolvedAt = report.ResolvedAt,
            ResolvedByUserId = report.ResolvedByUserId,
            ImageUrls = report.Images
                .Select(x => GetImageUrl(x.ImageUrl))
                .ToList()
        };
    }

    private static string GetImageUrl(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return string.Empty;
        }

        if (Uri.TryCreate(
                imageUrl,
                UriKind.Absolute,
                out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp ||
             uri.Scheme == Uri.UriSchemeHttps))
        {
            return imageUrl;
        }

        var fileName = Path.GetFileName(
            imageUrl.Replace('\\', '/'));

        return $"/uploads/reports/{fileName}";
    }

    public static IEnumerable<ReportResponse> ToResponse(
        IEnumerable<Report> reports)
    {
        return reports.Select(ToResponse);
    }

    public static IEnumerable<AdminReportResponse> ToAdminResponse(
        IEnumerable<Report> reports)
    {
        return reports.Select(ToAdminResponse);
    }

    public static void Approve(
        Report report,
        int approvedByUserId)
    {
        report.Status = ReportStatus.Approved;
        report.ApprovedByUserId = approvedByUserId;
        report.ApprovedAt = DateTimeOffset.UtcNow;
    }

    public static void Reject(
        Report report)
    {
        report.Status = ReportStatus.Rejected;
        report.UpdatedAt = DateTimeOffset.UtcNow;
    }

    public static void Resolve(
        Report report,
        int resolvedByUserId)
    {
        report.Status = ReportStatus.Resolved;
        report.ResolvedAt = DateTimeOffset.UtcNow;
        report.ResolvedByUserId = resolvedByUserId;
    }
}