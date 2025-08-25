using Elevator.Domain.Enums;

namespace Elevator.Application.DTOs;

/// <summary>
/// Data transfer object for elevator status information
/// </summary>
public class ElevatorStatusDto
{
    /// <summary>
    /// Gets or sets the elevator identifier
    /// </summary>
    public string ElevatorId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the current floor
    /// </summary>
    public int CurrentFloor { get; set; }
    
    /// <summary>
    /// Gets or sets the current direction
    /// </summary>
    public ElevatorDirection Direction { get; set; }
    
    /// <summary>
    /// Gets or sets the current state
    /// </summary>
    public ElevatorState State { get; set; }
    
    /// <summary>
    /// Gets or sets the number of current passengers
    /// </summary>
    public int CurrentPassengers { get; set; }
    
    /// <summary>
    /// Gets or sets the maximum passenger capacity
    /// </summary>
    public int MaxPassengers { get; set; }
    
    /// <summary>
    /// Gets or sets the elevator type
    /// </summary>
    public ElevatorType Type { get; set; }
    
    /// <summary>
    /// Gets or sets the door state
    /// </summary>
    public DoorState DoorState { get; set; }
    
    /// <summary>
    /// Gets or sets whether the elevator is under maintenance
    /// </summary>
    public bool IsUnderMaintenance { get; set; }
    
    /// <summary>
    /// Gets or sets the list of destination floors
    /// </summary>
    public List<int> DestinationFloors { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the estimated time to next destination in seconds
    /// </summary>
    public double EstimatedTimeToNextDestination { get; set; }
}
