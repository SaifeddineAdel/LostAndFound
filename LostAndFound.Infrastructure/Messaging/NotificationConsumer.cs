using System.Text.Json;
using LostAndFound.Application.Features.Notifications.Interfaces;
using LostAndFound.Application.Messaging.Events;
using LostAndFound.Domain.Entities;
using LostAndFound.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LostAndFound.Infrastructure.Messaging;

public sealed class NotificationConsumer : BackgroundService
{
    private const string AdminRole = "Admin";

    private readonly RabbitMqOptions _options;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NotificationConsumer> _logger;

    public NotificationConsumer(
        IOptions<RabbitMqOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<NotificationConsumer> logger)
    {
        _options = options.Value;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
        };

        await using var connection =
            await factory.CreateConnectionAsync(stoppingToken);

        await using var channel =
            await connection.CreateChannelAsync(
                cancellationToken: stoppingToken);

        await channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: _options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);

        await channel.QueueBindAsync(
            queue: _options.QueueName,
            exchange: _options.ExchangeName,
            routingKey: "report.created",
            cancellationToken: stoppingToken);

        await channel.QueueBindAsync(
            queue: _options.QueueName,
            exchange: _options.ExchangeName,
            routingKey: "report.approved",
            cancellationToken: stoppingToken);

        await channel.QueueBindAsync(
            queue: _options.QueueName,
            exchange: _options.ExchangeName,
            routingKey: "report.rejected",
            cancellationToken: stoppingToken);

        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            try
            {
                await ProcessMessageAsync(
                    eventArgs.RoutingKey,
                    eventArgs.Body.ToArray(),
                    stoppingToken);

                await channel.BasicAckAsync(
                    deliveryTag: eventArgs.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);

                _logger.LogInformation(
                    "Processed notification event {RoutingKey}",
                    eventArgs.RoutingKey);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Failed to process event {RoutingKey}",
                    eventArgs.RoutingKey);

                try
                {
                    await channel.BasicNackAsync(
                        deliveryTag: eventArgs.DeliveryTag,
                        multiple: false,
                        requeue: true,
                        cancellationToken: stoppingToken);
                }
                catch (Exception nackException)
                {
                    _logger.LogError(
                        nackException,
                        "Failed to reject RabbitMQ message.");
                }
            }
        };

        await channel.BasicConsumeAsync(
            queue: _options.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        _logger.LogInformation(
            "Notification consumer started on queue {QueueName}",
            _options.QueueName);

        try
        {
            await Task.Delay(
                Timeout.InfiniteTimeSpan,
                stoppingToken);
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            // The application is shutting down.
        }
    }

    private async Task ProcessMessageAsync(
        string routingKey,
        byte[] body,
        CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var repository = scope.ServiceProvider
            .GetRequiredService<INotificationRepository>();

        switch (routingKey)
        {
            case "report.created":
                {
                    var message =
                        JsonSerializer.Deserialize<ReportCreatedEvent>(body)
                        ?? throw new InvalidOperationException(
                            "Invalid ReportCreatedEvent message.");

                    var userManager = scope.ServiceProvider
                        .GetRequiredService<UserManager<ApplicationUser>>();

                    var admins =
                        await userManager.GetUsersInRoleAsync(AdminRole);

                    foreach (var admin in admins)
                    {
                        await repository.AddAsync(
                            new Notification
                            {
                                UserId = admin.Id,
                                ReportId = message.ReportId,
                                Title = "New report submitted",
                                Message =
                                    $"Report #{message.ReportId}: " +
                                    $"{message.Title} is waiting for review.",
                                IsRead = false,
                                CreatedAt = DateTimeOffset.UtcNow
                            },
                            cancellationToken);
                    }

                    if (admins.Count == 0)
                    {
                        _logger.LogWarning(
                            "No users found in the {AdminRole} role.",
                            AdminRole);
                    }

                    await repository.SaveChangesAsync(cancellationToken);
                    break;
                }

            case "report.approved":
                {
                    var message =
                        JsonSerializer.Deserialize<ReportApprovedEvent>(body)
                        ?? throw new InvalidOperationException(
                            "Invalid ReportApprovedEvent message.");

                    await repository.AddAsync(
                        new Notification
                        {
                            UserId = message.UserId,
                            ReportId = message.ReportId,
                            Title = "Report approved",
                            Message =
                                $"Your report #{message.ReportId} has been approved.",
                            IsRead = false,
                            CreatedAt = DateTimeOffset.UtcNow
                        },
                        cancellationToken);

                    await repository.SaveChangesAsync(cancellationToken);
                    break;
                }

            case "report.rejected":
                {
                    var message =
                        JsonSerializer.Deserialize<ReportRejectedEvent>(body)
                        ?? throw new InvalidOperationException(
                            "Invalid ReportRejectedEvent message.");

                    await repository.AddAsync(
                        new Notification
                        {
                            UserId = message.UserId,
                            ReportId = message.ReportId,
                            Title = "Report rejected",
                            Message =
                                $"Your report #{message.ReportId} was rejected. " +
                                $"Reason: {message.RejectionReason}",
                            IsRead = false,
                            CreatedAt = DateTimeOffset.UtcNow
                        },
                        cancellationToken);

                    await repository.SaveChangesAsync(cancellationToken);
                    break;
                }

            default:
                throw new InvalidOperationException(
                    $"Unsupported routing key: {routingKey}");
        }
    }
}
