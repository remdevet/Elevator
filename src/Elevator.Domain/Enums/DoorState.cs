namespace Elevator.Domain.Enums;

/// <summary>
/// Represents the state of elevator doors
/// </summary>
public enum DoorState
{
    /// <summary>
    /// Doors are fully closed
    /// </summary>
    Closed = 0,
    
    /// <summary>
    /// Doors are fully open
    /// </summary>
    Open = 1,
    
    /// <summary>
    /// Doors are in the process of opening
    /// </summary>
    Opening = 2,
    
    /// <summary>
    /// Doors are in the process of closing
    /// </summary>
    Closing = 3,
    
    /// <summary>
    /// Doors are stuck or malfunctioning
    /// </summary>
    Stuck = 4
}
