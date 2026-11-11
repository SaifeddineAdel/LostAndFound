namespace LostAndFound.Application.Messaging;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(
        string routingKey,
        TEvent message,
        CancellationToken cancellationToken = default);
}