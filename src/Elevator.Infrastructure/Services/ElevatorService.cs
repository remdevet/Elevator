using Elevator.Domain.Entities;
using Elevator.Domain.Enums;
using Elevator.Domain.Interfaces;
using Elevator.Domain.Events;
using Common.Messaging.Interfaces;
using Microsoft.Extensions.Logging;

namespace Elevator.Infrastructure.Services;

/// <summary>
/// Implementation of the elevator service
/// </summary>
public class ElevatorService : IElevatorService
{
    private readonly ILogger<ElevatorService> _logger;
    private readonly IMessageBus _messageBus;
    private readonly List<Elevator.Domain.Entities.Elevator> _elevators;
    private readonly List<Floor> _floors;
    private readonly object _lockObject = new();
    
    public ElevatorService(ILogger<ElevatorService> logger, IMessageBus messageBus)
    {
        _logger = logger;
        _messageBus = messageBus;
        _elevators = new List<Elevator.Domain.Entities.Elevator>();
        _floors = new List<Floor>();
        
        InitializeElevators();
        InitializeFloors();
    }
    
    /// <summary>
    /// Calls an elevator to a specific floor
    /// </summary>
    public async Task<Elevator.Domain.Entities.Elevator?> CallElevatorAsync(int floorNumber, ElevatorDirection direction, int peopleWaiting)
    {
        try
        {
            _logger.LogInformation("Calling elevator to floor {FloorNumber} in direction {Direction} with {PeopleWaiting} people waiting", 
                floorNumber, direction, peopleWaiting);
            
            // Validate floor number
            if (floorNumber < 1 || floorNumber > _floors.Count)
            {
                _logger.LogWarning("Invalid floor number: {FloorNumber}", floorNumber);
                return null;
            }
            
            // Update floor information
            var floor = _floors.FirstOrDefault(f => f.FloorNumber == floorNumber);
            if (floor != null)
            {
                floor.AddPeopleWaiting(peopleWaiting);
                
                if (direction == ElevatorDirection.Up)
                    floor.PressUpButton();
                else if (direction == ElevatorDirection.Down)
                    floor.PressDownButton();
            }
            
            // Get the nearest available elevator
            var elevator = await GetNearestAvailableElevatorAsync(floorNumber, direction);
            
            if (elevator != null)
            {
                // Add destination floor to elevator
                elevator.AddDestinationFloor(floorNumber);
                
                // Publish event
                var elevatorCalledEvent = new ElevatorCalledEvent(floorNumber, direction.ToString(), peopleWaiting);
                await _messageBus.PublishAsync(elevatorCalledEvent);
                
                _logger.LogInformation("Elevator {ElevatorId} assigned to floor {FloorNumber}", 
                    elevator.ElevatorId, floorNumber);
                
                return elevator;
            }
            else
            {
                _logger.LogWarning("No available elevator found for floor {FloorNumber}", floorNumber);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling elevator to floor {FloorNumber}", floorNumber);
            return null;
        }
    }
    
    /// <summary>
    /// Gets the nearest available elevator to a specific floor
    /// </summary>
    public async Task<Elevator.Domain.Entities.Elevator?> GetNearestAvailableElevatorAsync(int floorNumber, ElevatorDirection direction)
    {
        try
        {
            var availableElevators = _elevators
                .Where(e => !e.IsUnderMaintenance && e.State != ElevatorState.OutOfService)
                .ToList();
            
            if (!availableElevators.Any())
                return null;
            
            // Find the best elevator based on multiple criteria
            var bestElevator = availableElevators
                .OrderBy(e => GetElevatorScore(e, floorNumber, direction))
                .FirstOrDefault();
            
            return bestElevator;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting nearest available elevator for floor {FloorNumber}", floorNumber);
            return null;
        }
    }
    
    /// <summary>
    /// Gets all elevators in the system
    /// </summary>
    public async Task<IEnumerable<Elevator.Domain.Entities.Elevator>> GetAllElevatorsAsync()
    {
        return await Task.FromResult(_elevators.AsReadOnly());
    }
    
    /// <summary>
    /// Gets elevator status information
    /// </summary>
    public async Task<Elevator.Domain.Entities.Elevator?> GetElevatorStatusAsync(string elevatorId)
    {
        return await Task.FromResult(_elevators.FirstOrDefault(e => e.ElevatorId == elevatorId));
    }
    
    /// <summary>
    /// Processes elevator movement simulation
    /// </summary>
    public async Task ProcessElevatorMovementAsync()
    {
        try
        {
            lock (_lockObject)
            {
                foreach (var elevator in _elevators.Where(e => !e.IsUnderMaintenance))
                {
                    ProcessElevatorMovement(elevator);
                }
            }
            
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing elevator movement");
        }
    }
    
    /// <summary>
    /// Sets elevator maintenance mode
    /// </summary>
    public async Task<bool> SetElevatorMaintenanceAsync(string elevatorId, bool isUnderMaintenance)
    {
        try
        {
            var elevator = _elevators.FirstOrDefault(e => e.ElevatorId == elevatorId);
            if (elevator == null)
                return false;
            
            if (isUnderMaintenance)
                elevator.SetUnderMaintenance();
            else
                elevator.RemoveFromMaintenance();
            
            _logger.LogInformation("Elevator {ElevatorId} maintenance status set to: {IsUnderMaintenance}", 
                elevatorId, isUnderMaintenance);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting maintenance status for elevator {ElevatorId}", elevatorId);
            return false;
        }
    }
    
    private void InitializeElevators()
    {
        // Create multiple elevators with different configurations
        _elevators.Add(new Elevator.Domain.Entities.Elevator("E1", 8, ElevatorType.Standard, 1.0, 3.0));
        _elevators.Add(new Elevator.Domain.Entities.Elevator("E2", 10, ElevatorType.HighSpeed, 2.0, 2.5));
        _elevators.Add(new Elevator.Domain.Entities.Elevator("E3", 6, ElevatorType.Glass, 1.2, 3.5));
        _elevators.Add(new Elevator.Domain.Entities.Elevator("E4", 12, ElevatorType.Freight, 0.8, 4.0));
        
        _logger.LogInformation("Initialized {Count} elevators", _elevators.Count);
    }
    
    private void InitializeFloors()
    {
        // Create floors (assuming a 20-story building)
        for (int i = 1; i <= 20; i++)
        {
            var maxCapacity = i == 1 ? 200 : 100; // Ground floor has higher capacity
            var isAccessible = i <= 15; // Upper floors might be restricted
            
            _floors.Add(new Floor(i, maxCapacity, isAccessible));
        }
        
        _logger.LogInformation("Initialized {Count} floors", _floors.Count);
    }
    
    private void ProcessElevatorMovement(Elevator.Domain.Entities.Elevator elevator)
    {
        if (elevator.State == ElevatorState.Idle && elevator.DestinationFloors.Count > 0)
        {
            // Open doors if elevator is at a destination floor
            var currentFloor = elevator.CurrentFloor;
            if (elevator.DestinationFloors.Contains(currentFloor))
            {
                elevator.OpenDoors();
                elevator.RemoveDestinationFloor(currentFloor);
                
                // Publish arrival event
                var arrivalEvent = new ElevatorArrivedEvent(
                    elevator.ElevatorId, 
                    currentFloor, 
                    elevator.Direction.ToString(), 
                    elevator.CurrentPassengers);
                
                _ = Task.Run(async () => await _messageBus.PublishAsync(arrivalEvent));
                
                _logger.LogDebug("Elevator {ElevatorId} arrived at floor {FloorNumber}", 
                    elevator.ElevatorId, currentFloor);
            }
            else
            {
                // Move to next destination
                elevator.MoveToNextDestination();
            }
        }
        else if (elevator.State == ElevatorState.DoorsOpen)
        {
            // Simulate door operation time
            // In a real implementation, this would use timers
            elevator.CloseDoors();
        }
        else if (elevator.State == ElevatorState.Moving)
        {
            // Elevator is already moving, continue to next destination
            elevator.MoveToNextDestination();
        }
    }
    
    private double GetElevatorScore(Elevator.Domain.Entities.Elevator elevator, int targetFloor, ElevatorDirection requestedDirection)
    {
        var distance = Math.Abs(elevator.CurrentFloor - targetFloor);
        var directionMatch = 0.0;
        var capacityAvailable = elevator.MaxPassengers - elevator.CurrentPassengers;
        var stateScore = 0.0;
        
        // Direction matching score
        if (elevator.Direction == ElevatorDirection.None)
            directionMatch = 1.0; // Idle elevator gets highest score
        else if (elevator.Direction == requestedDirection)
            directionMatch = 0.5; // Same direction gets good score
        else
            directionMatch = 2.0; // Opposite direction gets penalty
        
        // State score
        switch (elevator.State)
        {
            case ElevatorState.Idle:
                stateScore = 0.0; // Best score
                break;
            case ElevatorState.DoorsOpen:
                stateScore = 0.5; // Good score
                break;
            case ElevatorState.Moving:
                stateScore = 1.0; // Moving gets penalty
                break;
            default:
                stateScore = 3.0; // Other states get high penalty
                break;
        }
        
        // Calculate final score (lower is better)
        var score = distance + directionMatch + stateScore;
        
        // Penalty for capacity issues
        if (capacityAvailable <= 0)
            score += 10.0;
        
        return score;
    }
}
