
using LostAndFound.Domain.Exceptions;

namespace LostAndFound.Domain.Entities;

public class ReportImage
{
    public int Id { get; private set; }

    public int ReportId { get; private set; }

    public string ImageUrl { get; private set; } = string.Empty;

    public Report Report { get; private set; } = null!;

    private ReportImage()
    {
    }

    private ReportImage(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            throw new DomainException("Image URL cannot be empty.");
        }

        ImageUrl = imageUrl.Trim();
    }

    public static ReportImage Create(string imageUrl)
    {
        return new ReportImage(imageUrl);
    }
}
