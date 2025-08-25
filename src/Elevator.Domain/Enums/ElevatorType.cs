namespace Elevator.Domain.Enums;

/// <summary>
/// Represents different types of elevators
/// </summary>
public enum ElevatorType
{
    /// <summary>
    /// Standard passenger elevator
    /// </summary>
    Standard = 0,
    
    /// <summary>
    /// High-speed elevator for tall buildings
    /// </summary>
    HighSpeed = 1,
    
    /// <summary>
    /// Glass elevator with transparent walls
    /// </summary>
    Glass = 2,
    
    /// <summary>
    /// Freight elevator for cargo and equipment
    /// </summary>
    Freight = 3,
    
    /// <summary>
    /// Service elevator for maintenance staff
    /// </summary>
    Service = 4,
    
    /// <summary>
    /// Express elevator that skips certain floors
    /// </summary>
    Express = 5
}
