using Elevator.Domain.Entities;
using Elevator.Domain.Enums;

namespace Elevator.Domain.Interfaces;

/// <summary>
/// Service interface for elevator operations
/// </summary>
public interface IElevatorService
{
    /// <summary>
    /// Calls an elevator to a specific floor
    /// </summary>
    /// <param name="floorNumber">The floor number to call the elevator to</param>
    /// <param name="direction">The direction requested (up or down)</param>
    /// <param name="peopleWaiting">Number of people waiting on the floor</param>
    /// <returns>The assigned elevator or null if none available</returns>
    Task<Elevator.Domain.Entities.Elevator?> CallElevatorAsync(int floorNumber, ElevatorDirection direction, int peopleWaiting);
    
    /// <summary>
    /// Gets the nearest available elevator to a specific floor
    /// </summary>
    /// <param name="floorNumber">The floor number</param>
    /// <param name="direction">The direction requested</param>
    /// <returns>The nearest available elevator or null if none available</returns>
    Task<Elevator.Domain.Entities.Elevator?> GetNearestAvailableElevatorAsync(int floorNumber, ElevatorDirection direction);
    
    /// <summary>
    /// Gets all elevators in the system
    /// </summary>
    /// <returns>Collection of all elevators</returns>
    Task<IEnumerable<Elevator.Domain.Entities.Elevator>> GetAllElevatorsAsync();
    
    /// <summary>
    /// Gets elevator status information
    /// </summary>
    /// <param name="elevatorId">The elevator identifier</param>
    /// <returns>Elevator status or null if not found</returns>
    Task<Elevator.Domain.Entities.Elevator?> GetElevatorStatusAsync(string elevatorId);
    
    /// <summary>
    /// Processes elevator movement simulation
    /// </summary>
    /// <returns>Task representing the asynchronous operation</returns>
    Task ProcessElevatorMovementAsync();
    
    /// <summary>
    /// Sets elevator maintenance mode
    /// </summary>
    /// <param name="elevatorId">The elevator identifier</param>
    /// <param name="isUnderMaintenance">Whether the elevator is under maintenance</param>
    /// <returns>True if operation was successful, false otherwise</returns>
    Task<bool> SetElevatorMaintenanceAsync(string elevatorId, bool isUnderMaintenance);
}
