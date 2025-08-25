namespace Common.Messaging.Interfaces;

/// <summary>
/// Message bus interface for publishing and subscribing to events
/// </summary>
public interface IMessageBus
{
    /// <summary>
    /// Publishes a message to all subscribers
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    /// <param name="message">The message to publish</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task PublishAsync<T>(T message) where T : class;
    
    /// <summary>
    /// Subscribes to a message type
    /// </summary>
    /// <typeparam name="T">The message type to subscribe to</typeparam>
    /// <param name="handler">The handler for the message</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task SubscribeAsync<T>(Func<T, Task> handler) where T : class;
    
    /// <summary>
    /// Unsubscribes from a message type
    /// </summary>
    /// <typeparam name="T">The message type to unsubscribe from</typeparam>
    /// <param name="handler">The handler to unsubscribe</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task UnsubscribeAsync<T>(Func<T, Task> handler) where T : class;
}
