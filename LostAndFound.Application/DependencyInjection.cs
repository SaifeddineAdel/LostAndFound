using LostAndFound.Application.Features.Notifications.Interfaces;
using LostAndFound.Application.Features.Notifications.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LostAndFound.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<INotificationService, NotificationService>();

        return services;
    }
}