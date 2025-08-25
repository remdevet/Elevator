using Common.Messaging.Events;

namespace Elevator.Domain.Events;

/// <summary>
/// Event raised when an elevator is called to a specific floor
/// </summary>
public class ElevatorCalledEvent : BaseEvent
{
    /// <summary>
    /// Gets the floor number where the elevator was called
    /// </summary>
    public int FloorNumber { get; }
    
    /// <summary>
    /// Gets the direction requested (up or down)
    /// </summary>
    public string RequestedDirection { get; }
    
    /// <summary>
    /// Gets the number of people waiting
    /// </summary>
    public int PeopleWaiting { get; }
    
    /// <summary>
    /// Gets the timestamp when the call was made
    /// </summary>
    public DateTime CallTime { get; }
    
    public ElevatorCalledEvent(int floorNumber, string requestedDirection, int peopleWaiting)
    {
        FloorNumber = floorNumber;
        RequestedDirection = requestedDirection;
        PeopleWaiting = peopleWaiting;
        CallTime = DateTime.UtcNow;
    }
}
