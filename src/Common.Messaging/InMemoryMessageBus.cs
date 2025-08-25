using Common.Messaging.Interfaces;
using Microsoft.Extensions.Logging;

namespace Common.Messaging;

/// <summary>
/// In-memory implementation of the message bus
/// </summary>
public class InMemoryMessageBus : IMessageBus
{
    private readonly ILogger<InMemoryMessageBus> _logger;
    private readonly Dictionary<Type, List<Func<object, Task>>> _subscribers;
    private readonly object _lockObject = new();
    
    public InMemoryMessageBus(ILogger<InMemoryMessageBus> logger)
    {
        _logger = logger;
        _subscribers = new Dictionary<Type, List<Func<object, Task>>>();
    }
    
    /// <summary>
    /// Publishes a message to all subscribers
    /// </summary>
    public async Task PublishAsync<T>(T message) where T : class
    {
        try
        {
            var messageType = typeof(T);
            List<Func<object, Task>> handlers;
            
            lock (_lockObject)
            {
                if (_subscribers.ContainsKey(messageType))
                {
                    handlers = _subscribers[messageType].ToList();
                }
                else
                {
                    handlers = new List<Func<object, Task>>();
                }
            }
            
            if (handlers.Count > 0)
            {
                // Execute handlers asynchronously
                var tasks = handlers.Select(handler => handler(message));
                await Task.WhenAll(tasks);
                
                _logger.LogDebug("Published message of type {MessageType} to {HandlerCount} subscribers", 
                    messageType.Name, handlers.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing message of type {MessageType}", typeof(T).Name);
            throw;
        }
    }
    
    /// <summary>
    /// Subscribes to a message type
    /// </summary>
    public async Task SubscribeAsync<T>(Func<T, Task> handler) where T : class
    {
        try
        {
            var messageType = typeof(T);
            
            lock (_lockObject)
            {
                if (!_subscribers.ContainsKey(messageType))
                {
                    _subscribers[messageType] = new List<Func<object, Task>>();
                }
                
                // Convert the strongly-typed handler to a generic object handler
                Func<object, Task> genericHandler = obj => handler((T)obj);
                _subscribers[messageType].Add(genericHandler);
                
                _logger.LogDebug("Subscribed to message type {MessageType}", messageType.Name);
            }
            
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error subscribing to message type {MessageType}", typeof(T).Name);
            throw;
        }
    }
    
    /// <summary>
    /// Unsubscribes from a message type
    /// </summary>
    public async Task UnsubscribeAsync<T>(Func<T, Task> handler) where T : class
    {
        try
        {
            var messageType = typeof(T);
            
            lock (_lockObject)
            {
                if (_subscribers.ContainsKey(messageType))
                {
                    // Note: This is a simplified implementation
                    // In a real scenario, you'd need to store the original handler references
                    // to properly remove them
                    _logger.LogWarning("Unsubscribe operation not fully implemented for message type {MessageType}", messageType.Name);
                }
            }
            
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unsubscribing from message type {MessageType}", typeof(T).Name);
            throw;
        }
    }
    
    /// <summary>
    /// Gets the number of subscribers for a message type
    /// </summary>
    public int GetSubscriberCount<T>() where T : class
    {
        var messageType = typeof(T);
        
        lock (_lockObject)
        {
            return _subscribers.ContainsKey(messageType) ? _subscribers[messageType].Count : 0;
        }
    }
    
    /// <summary>
    /// Clears all subscribers
    /// </summary>
    public void ClearSubscribers()
    {
        lock (_lockObject)
        {
            _subscribers.Clear();
            _logger.LogInformation("All message subscribers cleared");
        }
    }
}
