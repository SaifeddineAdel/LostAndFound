using LostAndFound.Domain.Entities;
using LostAndFound.Domain.Enums;
using LostAndFound.Domain.Exceptions;

namespace LostAndFound.Tests.Domain;

public class ReportTests
{
    [Fact]
    public void Create_ValidData_ShouldStartAsPending()
    {
        // Arrange
        var itemDate = DateTimeOffset.UtcNow.AddDays(-1);

        // Act
        var report = CreateReport(itemDate: itemDate);

        // Assert
        Assert.Equal(ReportStatus.Pending, report.Status);
        Assert.Equal(ReportType.Lost, report.Type);
        Assert.Equal("Lost Wallet", report.Title);
        Assert.Equal("Black leather wallet", report.Description);
        Assert.Null(report.ApprovedAt);
        Assert.Equal(itemDate, report.ItemDate);
    }

    [Fact]
    public void Approve_AlreadyApprovedReport_ShouldThrowException()
    {
        // Arrange
        var report = CreateReport();
        report.Approve(1);

        // Act & Assert
        Assert.Throws<DomainException>(
            () => report.Approve(1));
    }

    [Fact]
    public void Report_ValidLifecycle_ShouldTransitionCorrectly()
    {
        // Arrange
        var report = CreateReport();

        // Act
        report.Approve(1);

        // Assert
        Assert.Equal(ReportStatus.Approved, report.Status);
        Assert.Equal(1, report.ApprovedByUserId);
        Assert.NotNull(report.ApprovedAt);

        // Act
        report.Resolve(1);

        // Assert
        Assert.Equal(ReportStatus.Resolved, report.Status);
        Assert.Equal(1, report.ResolvedByUserId);
        Assert.NotNull(report.ResolvedAt);
    }

    private static Report CreateReport(
        DateTimeOffset? itemDate = null)
    {
        return Report.Create(
            userId: 1,
            type: ReportType.Lost,
            title: "Lost Wallet",
            description: "Black leather wallet",
            privateDescription: "Small scratch inside",
            categoryId: 1,
            subCategoryId: 1,
            locationId: 1,
            itemDate: itemDate ?? DateTimeOffset.UtcNow.AddDays(-1));
    }
}