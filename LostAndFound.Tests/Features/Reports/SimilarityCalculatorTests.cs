using LostAndFound.Application.Features.Reports.Services;
using LostAndFound.Domain.Entities;
using LostAndFound.Domain.Enums;

namespace LostAndFound.Tests.Features.Reports;

public class SimilarityCalculatorTests
{
    private readonly SimilarityCalculator _calculator = new();

    [Fact]
    public void Calculate_IdenticalReports_ShouldReturn100()
    {
        // Arrange
        var first = CreateReport();
        var second = CreateReport();

        // Act
        var score = _calculator.Calculate(first, second);

        // Assert
        Assert.Equal(100, score);
    }

    [Fact]
    public void Calculate_DifferentReports_ShouldReturnLowScore()
    {
        // Arrange
        var first = CreateReport();

        var second = CreateReport(
            title: "Phone",
            description: "White smartphone",
            privateDescription: "Broken screen",
            locationId: 2,
            itemDate: DateTimeOffset.UtcNow.AddDays(-30));

        // Act
        var score = _calculator.Calculate(first, second);

        // Assert
        Assert.True(score < 30);
    }

    [Fact]
    public void Calculate_MissingPrivateDescription_ShouldRedistributeWeights()
    {
        // Arrange
        var first = CreateReport(privateDescription: null);
        var second = CreateReport();

        // Act
        var score = _calculator.Calculate(first, second);

        // Assert
        Assert.Equal(100, score);
    }

    private static Report CreateReport(
        string title = "Wallet",
        string description = "Black leather wallet",
        string? privateDescription = "Small scratch",
        int locationId = 1,
        DateTimeOffset? itemDate = null)
    {
        return Report.Create(
            userId: 1,
            type: ReportType.Lost,
            title: title,
            description: description,
            privateDescription: privateDescription,
            categoryId: 1,
            subCategoryId: 1,
            locationId: locationId,
            itemDate: itemDate ?? DateTimeOffset.UtcNow.AddDays(-1));
    }
}