namespace LostAndFound.Application.Messaging.Events;

public sealed record ReportApprovedEvent(
    int ReportId,
    int UserId,
    DateTimeOffset OccurredAt
);