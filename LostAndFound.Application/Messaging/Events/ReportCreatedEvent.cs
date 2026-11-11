namespace LostAndFound.Application.Messaging.Events;

public sealed record ReportCreatedEvent(
    int ReportId,
    int CreatedByUserId,
    string Title,
    DateTimeOffset OccurredAt
);