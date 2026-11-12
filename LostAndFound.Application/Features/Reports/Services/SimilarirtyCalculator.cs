using LostAndFound.Domain.Entities;

namespace LostAndFound.Application.Features.Reports.Services;

public sealed class SimilarityCalculator
{
    public double Calculate(Report first, Report second)
    {
        double score = 0;
        double totalWeight = 0;

        if (!string.IsNullOrWhiteSpace(first.PrivateDescription) &&
            !string.IsNullOrWhiteSpace(second.PrivateDescription))
        {
            score += CalculateTextSimilarity(
                first.PrivateDescription,
                second.PrivateDescription) * 0.40;

            totalWeight += 0.40;
        }

        score += CalculateTextSimilarity(
            first.Description,
            second.Description) * 0.25;

        totalWeight += 0.25;

        score += CalculateTextSimilarity(
            first.Title,
            second.Title) * 0.15;

        totalWeight += 0.15;

        score += (first.LocationId == second.LocationId ? 100 : 0) * 0.10;

        totalWeight += 0.10;

        score += CalculateDateSimilarity(
            first.ItemDate,
            second.ItemDate) * 0.10;

        totalWeight += 0.10;

        return score / totalWeight;
    }

    private double CalculateTextSimilarity(string first, string second)
    {
        var firstWords = first.ToLower().Split(
            ' ',
            StringSplitOptions.RemoveEmptyEntries);

        var secondWords = second.ToLower().Split(
            ' ',
            StringSplitOptions.RemoveEmptyEntries);

        int matchingWords = firstWords
            .Distinct()
            .Count(word => secondWords.Contains(word));

        int totalWords = Math.Max(
            firstWords.Distinct().Count(),
            secondWords.Distinct().Count());

        if (totalWords == 0)
            return 0;

        return (double)matchingWords / totalWords * 100;
    }

    private double CalculateDateSimilarity(DateTimeOffset first, DateTimeOffset second)
    {
        int days = Math.Abs((first.Date - second.Date).Days);

        if (days == 0) return 100;
        if (days <= 3) return 80;
        if (days <= 7) return 50;
        if (days <= 14) return 25;

        return 0;
    }
}