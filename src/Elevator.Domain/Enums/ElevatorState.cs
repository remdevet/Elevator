namespace Elevator.Domain.Enums;

/// <summary>
/// Represents the current state of an elevator
/// </summary>
public enum ElevatorState
{
    /// <summary>
    /// Elevator is idle and waiting for requests
    /// </summary>
    Idle = 0,
    
    /// <summary>
    /// Elevator is moving between floors
    /// </summary>
    Moving = 1,
    
    /// <summary>
    /// Elevator doors are open for passenger entry/exit
    /// </summary>
    DoorsOpen = 2,
    
    /// <summary>
    /// Elevator is under maintenance
    /// </summary>
    UnderMaintenance = 3,
    
    /// <summary>
    /// Elevator is out of service
    /// </summary>
    OutOfService = 4,
    
    /// <summary>
    /// Elevator is in emergency mode
    /// </summary>
    Emergency = 5
}
