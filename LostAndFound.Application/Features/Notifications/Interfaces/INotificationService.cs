using LostAndFound.Application.Features.Notifications.DTOs;

namespace LostAndFound.Application.Features.Notifications.Interfaces;

public interface INotificationService
{
    Task<List<NotificationResponse>> GetMyNotificationsAsync(
        int userId,
        CancellationToken cancellationToken = default);

    Task<int> GetUnreadCountAsync(
        int userId,
        CancellationToken cancellationToken = default);

    Task<bool> MarkAsReadAsync(
        int notificationId,
        int userId,
        CancellationToken cancellationToken = default);

    Task MarkAllAsReadAsync(
        int userId,
        CancellationToken cancellationToken = default);
}