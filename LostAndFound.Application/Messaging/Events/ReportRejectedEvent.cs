namespace LostAndFound.Application.Messaging.Events;

public sealed record ReportRejectedEvent(
    int ReportId,
    int UserId,
    string RejectionReason,
    DateTimeOffset OccurredAt
);