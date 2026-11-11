
using System.Text.Json;
using LostAndFound.Application.Messaging;
using LostAndFound.Application.Messaging.Events;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace LostAndFound.Infrastructure.Messaging;

public sealed class RabbitMqEventPublisher
    : IEventPublisher, IAsyncDisposable
{
    private readonly RabbitMqOptions _options;

    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private IConnection? _connection;
    private IChannel? _channel;

    private static readonly string[] RoutingKeys =
    [
        "report.created",
        "report.approved",
        "report.rejected"
    ];

    public RabbitMqEventPublisher(
        IOptions<RabbitMqOptions> options)
    {
        _options = options.Value;
    }

    public async Task PublishAsync<TEvent>(
        string routingKey,
        TEvent message,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (!RoutingKeys.Contains(routingKey))
        {
            throw new ArgumentException(
                $"Unsupported routing key: {routingKey}",
                nameof(routingKey));
        }

        await _semaphore.WaitAsync(cancellationToken);

        try
        {
            await EnsureInitializedAsync(cancellationToken);

            var body = JsonSerializer.SerializeToUtf8Bytes(message);

            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json",
                MessageId = Guid.NewGuid().ToString(),
                Type = typeof(TEvent).Name
            };

            await _channel!.BasicPublishAsync(
                exchange: _options.ExchangeName,
                routingKey: routingKey,
                mandatory: true,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task EnsureInitializedAsync(
        CancellationToken cancellationToken)
    {
        if (_connection is null || !_connection.IsOpen)
        {
            if (_channel is not null)
            {
                await _channel.DisposeAsync();
                _channel = null;
            }

            if (_connection is not null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }

            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
            };

            _connection = await factory.CreateConnectionAsync(
                cancellationToken);
        }

        if (_channel is null || !_channel.IsOpen)
        {
            if (_channel is not null)
            {
                await _channel.DisposeAsync();
            }

            var channelOptions = new CreateChannelOptions(
                publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true);

            _channel = await _connection.CreateChannelAsync(
                channelOptions,
                cancellationToken);

            await _channel.ExchangeDeclareAsync(
                exchange: _options.ExchangeName,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);

            await _channel.QueueDeclareAsync(
                queue: _options.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellationToken);

            foreach (var key in RoutingKeys)
            {
                await _channel.QueueBindAsync(
                    queue: _options.QueueName,
                    exchange: _options.ExchangeName,
                    routingKey: key,
                    cancellationToken: cancellationToken);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _semaphore.WaitAsync();

        try
        {
            if (_channel is not null)
            {
                await _channel.DisposeAsync();
                _channel = null;
            }

            if (_connection is not null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }
        }
        finally
        {
            _semaphore.Release();
            _semaphore.Dispose();
        }
    }
}
