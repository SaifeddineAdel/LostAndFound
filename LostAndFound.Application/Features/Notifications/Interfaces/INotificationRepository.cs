using LostAndFound.Domain.Entities;

namespace LostAndFound.Application.Features.Notifications.Interfaces;

public interface INotificationRepository
{
    Task AddAsync(
        Notification notification,
        CancellationToken cancellationToken = default);

    Task<List<Notification>> GetByUserIdAsync(
        int userId,
        CancellationToken cancellationToken = default);

    Task<Notification?> GetByIdAndUserIdAsync(
        int notificationId,
        int userId,
        CancellationToken cancellationToken = default);

    Task<int> GetUnreadCountAsync(
        int userId,
        CancellationToken cancellationToken = default);

    Task MarkAllAsReadAsync(
        int userId,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}