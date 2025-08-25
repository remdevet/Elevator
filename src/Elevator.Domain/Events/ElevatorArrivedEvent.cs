using Common.Messaging.Events;

namespace Elevator.Domain.Events;

/// <summary>
/// Event raised when an elevator arrives at a specific floor
/// </summary>
public class ElevatorArrivedEvent : BaseEvent
{
    /// <summary>
    /// Gets the elevator identifier
    /// </summary>
    public string ElevatorId { get; }
    
    /// <summary>
    /// Gets the floor number where the elevator arrived
    /// </summary>
    public int FloorNumber { get; }
    
    /// <summary>
    /// Gets the direction the elevator was traveling
    /// </summary>
    public string Direction { get; }
    
    /// <summary>
    /// Gets the number of passengers in the elevator
    /// </summary>
    public int Passengers { get; }
    
    /// <summary>
    /// Gets the timestamp when the elevator arrived
    /// </summary>
    public DateTime ArrivalTime { get; }
    
    public ElevatorArrivedEvent(string elevatorId, int floorNumber, string direction, int passengers)
    {
        ElevatorId = elevatorId;
        FloorNumber = floorNumber;
        Direction = direction;
        Passengers = passengers;
        ArrivalTime = DateTime.UtcNow;
    }
}
