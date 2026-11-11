namespace LostAndFound.Domain.Entities;

public class Notification
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? ReportId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;

    public DateTimeOffset CreatedAt { get; set; }
        = DateTimeOffset.UtcNow;
}