namespace Elevator.Domain.Enums;

/// <summary>
/// Represents the direction of elevator movement
/// </summary>
public enum ElevatorDirection
{
    /// <summary>
    /// Elevator is not moving
    /// </summary>
    None = 0,
    
    /// <summary>
    /// Elevator is moving upward
    /// </summary>
    Up = 1,
    
    /// <summary>
    /// Elevator is moving downward
    /// </summary>
    Down = 2
}
