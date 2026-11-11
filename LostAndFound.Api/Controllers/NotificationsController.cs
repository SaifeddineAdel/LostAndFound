using LostAndFound.Application.Common.Interfaces;
using LostAndFound.Application.Features.Notifications.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LostAndFound.Api.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public sealed class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserService _currentUserService;

    public NotificationsController(
        INotificationService notificationService,
        ICurrentUserService currentUserService)
    {
        _notificationService = notificationService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyNotifications(
        CancellationToken cancellationToken)
    {
        var notifications =
            await _notificationService.GetMyNotificationsAsync(
                _currentUserService.UserId,
                cancellationToken);

        return Ok(notifications);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount(
        CancellationToken cancellationToken)
    {
        var count = await _notificationService.GetUnreadCountAsync(
            _currentUserService.UserId,
            cancellationToken);

        return Ok(new { unreadCount = count });
    }

    [HttpPatch("{id:int}/read")]
    public async Task<IActionResult> MarkAsRead(
        int id,
        CancellationToken cancellationToken)
    {
        var success = await _notificationService.MarkAsReadAsync(
            id,
            _currentUserService.UserId,
            cancellationToken);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllAsRead(
        CancellationToken cancellationToken)
    {
        await _notificationService.MarkAllAsReadAsync(
            _currentUserService.UserId,
            cancellationToken);

        return NoContent();
    }
}