namespace LostAndFound.Domain.Entities;

public class ReportImage
{
    public int Id { get; set; }

    public int ReportId { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public Report Report { get; set; } = null!;
}