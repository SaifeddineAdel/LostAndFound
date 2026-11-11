namespace LostAndFound.Application.Features.Notifications.DTOs;

public sealed record NotificationResponse(
    int Id,
    int? ReportId,
    string Title,
    string Message,
    bool IsRead,
    DateTimeOffset CreatedAt);