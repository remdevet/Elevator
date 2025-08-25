namespace Common.Messaging.Events;

/// <summary>
/// Base class for all domain events
/// </summary>
public abstract class BaseEvent
{
    /// <summary>
    /// Gets the event identifier
    /// </summary>
    public Guid EventId { get; }
    
    /// <summary>
    /// Gets the timestamp when the event occurred
    /// </summary>
    public DateTime OccurredOn { get; }
    
    /// <summary>
    /// Gets the event type name
    /// </summary>
    public string EventType => GetType().Name;
    
    protected BaseEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }
}
