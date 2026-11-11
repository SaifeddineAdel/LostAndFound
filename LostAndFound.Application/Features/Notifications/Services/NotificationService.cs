using LostAndFound.Application.Features.Notifications.DTOs;
using LostAndFound.Application.Features.Notifications.Interfaces;

namespace LostAndFound.Application.Features.Notifications.Services;

public sealed class NotificationService : INotificationService
{
    private readonly INotificationRepository _repository;

    public NotificationService(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<NotificationResponse>> GetMyNotificationsAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var notifications = await _repository.GetByUserIdAsync(
            userId,
            cancellationToken);

        return notifications
            .Select(n => new NotificationResponse(
                n.Id,
                n.ReportId,
                n.Title,
                n.Message,
                n.IsRead,
                n.CreatedAt))
            .ToList();
    }

    public Task<int> GetUnreadCountAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetUnreadCountAsync(
            userId,
            cancellationToken);
    }

    public async Task<bool> MarkAsReadAsync(
        int notificationId,
        int userId,
        CancellationToken cancellationToken = default)
    {
        var notification = await _repository.GetByIdAndUserIdAsync(
            notificationId,
            userId,
            cancellationToken);

        if (notification is null)
        {
            return false;
        }

        if (!notification.IsRead)
        {
            notification.IsRead = true;

            await _repository.SaveChangesAsync(cancellationToken);
        }

        return true;
    }

    public async Task MarkAllAsReadAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        await _repository.MarkAllAsReadAsync(
            userId,
            cancellationToken);
    }
}