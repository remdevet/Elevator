using Common.Dry.Base;
using Common.Messaging.Events;
using Elevator.Domain.Enums;
using Elevator.Domain.Events;

namespace Elevator.Domain.Entities;

/// <summary>
/// Represents an elevator in the building
/// </summary>
public class Elevator : EntityBase
{
    /// <summary>
    /// Gets or sets the elevator identifier
    /// </summary>
    public string ElevatorId { get; set; }
    
    /// <summary>
    /// Gets or sets the current floor where the elevator is located
    /// </summary>
    public int CurrentFloor { get; private set; }
    
    /// <summary>
    /// Gets or sets the current direction of movement
    /// </summary>
    public ElevatorDirection Direction { get; private set; }
    
    /// <summary>
    /// Gets or sets the current state of the elevator
    /// </summary>
    public ElevatorState State { get; private set; }
    
    /// <summary>
    /// Gets or sets the number of passengers currently in the elevator
    /// </summary>
    public int CurrentPassengers { get; private set; }
    
    /// <summary>
    /// Gets or sets the maximum passenger capacity
    /// </summary>
    public int MaxPassengers { get; set; }
    
    /// <summary>
    /// Gets or sets the list of floors to visit
    /// </summary>
    public List<int> DestinationFloors { get; private set; }
    
    /// <summary>
    /// Gets or sets the elevator type
    /// </summary>
    public ElevatorType Type { get; set; }
    
    /// <summary>
    /// Gets or sets the speed in floors per second
    /// </summary>
    public double Speed { get; set; }
    
    /// <summary>
    /// Gets or sets the time to open/close doors in seconds
    /// </summary>
    public double DoorOperationTime { get; set; }
    
    /// <summary>
    /// Gets or sets the current door state
    /// </summary>
    public DoorState DoorState { get; private set; }
    
    /// <summary>
    /// Gets or sets the maintenance status
    /// </summary>
    public bool IsUnderMaintenance { get; set; }
    
    /// <summary>
    /// Gets or sets the last maintenance date
    /// </summary>
    public DateTime? LastMaintenanceDate { get; set; }
    
    /// <summary>
    /// Gets or sets the total trips made by this elevator
    /// </summary>
    public int TotalTrips { get; private set; }
    
    /// <summary>
    /// Gets or sets the total distance traveled in floors
    /// </summary>
    public int TotalDistanceTraveled { get; private set; }
    
    public Elevator(string elevatorId, int maxPassengers = 8, ElevatorType type = ElevatorType.Standard, 
                   double speed = 1.0, double doorOperationTime = 3.0)
    {
        ElevatorId = elevatorId;
        CurrentFloor = 1;
        Direction = ElevatorDirection.None;
        State = ElevatorState.Idle;
        CurrentPassengers = 0;
        MaxPassengers = maxPassengers;
        DestinationFloors = new List<int>();
        Type = type;
        Speed = speed;
        DoorOperationTime = doorOperationTime;
        DoorState = DoorState.Closed;
        IsUnderMaintenance = false;
        TotalTrips = 0;
        TotalDistanceTraveled = 0;
    }
    
    /// <summary>
    /// Adds a destination floor to the elevator's route
    /// </summary>
    /// <param name="floorNumber">The floor number to add</param>
    /// <returns>True if added successfully, false otherwise</returns>
    public bool AddDestinationFloor(int floorNumber)
    {
        if (!DestinationFloors.Contains(floorNumber))
        {
            DestinationFloors.Add(floorNumber);
            ModifiedAt = DateTime.UtcNow;
            Version++;
            
            // Update direction if elevator is idle
            if (State == ElevatorState.Idle)
            {
                UpdateDirection();
            }
            
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Removes a destination floor from the elevator's route
    /// </summary>
    /// <param name="floorNumber">The floor number to remove</param>
    /// <returns>True if removed successfully, false otherwise</returns>
    public bool RemoveDestinationFloor(int floorNumber)
    {
        if (DestinationFloors.Remove(floorNumber))
        {
            ModifiedAt = DateTime.UtcNow;
            Version++;
            
            // Update direction if no more destinations
            if (DestinationFloors.Count == 0)
            {
                Direction = ElevatorDirection.None;
                State = ElevatorState.Idle;
            }
            
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Moves the elevator to the next destination
    /// </summary>
    /// <returns>True if movement was successful, false otherwise</returns>
    public bool MoveToNextDestination()
    {
        if (DestinationFloors.Count == 0 || State == ElevatorState.UnderMaintenance)
            return false;
        
        // Sort destinations based on current direction
        var sortedDestinations = GetSortedDestinations();
        
        if (sortedDestinations.Count > 0)
        {
            var nextFloor = sortedDestinations[0];
            var distance = Math.Abs(nextFloor - CurrentFloor);
            
            if (distance > 0)
            {
                // Update state and direction
                State = ElevatorState.Moving;
                Direction = nextFloor > CurrentFloor ? ElevatorDirection.Up : ElevatorDirection.Down;
                
                // Move to next floor
                CurrentFloor = nextFloor;
                DestinationFloors.Remove(nextFloor);
                
                // Update statistics
                TotalDistanceTraveled += distance;
                TotalTrips++;
                
                ModifiedAt = DateTime.UtcNow;
                Version++;
                
                // Update direction after movement
                UpdateDirection();
                
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Opens the elevator doors
    /// </summary>
    /// <returns>True if doors opened successfully, false otherwise</returns>
    public bool OpenDoors()
    {
        if (State == ElevatorState.Moving || DoorState == DoorState.Open)
            return false;
        
        DoorState = DoorState.Open;
        State = ElevatorState.DoorsOpen;
        ModifiedAt = DateTime.UtcNow;
        Version++;
        
        return true;
    }
    
    /// <summary>
    /// Closes the elevator doors
    /// </summary>
    /// <returns>True if doors closed successfully, false otherwise</returns>
    public bool CloseDoors()
    {
        if (DoorState == DoorState.Closed)
            return false;
        
        DoorState = DoorState.Closed;
        State = DestinationFloors.Count > 0 ? ElevatorState.Idle : ElevatorState.Idle;
        ModifiedAt = DateTime.UtcNow;
        Version++;
        
        return true;
    }
    
    /// <summary>
    /// Adds passengers to the elevator
    /// </summary>
    /// <param name="count">Number of passengers to add</param>
    /// <returns>True if passengers added successfully, false if capacity exceeded</returns>
    public bool AddPassengers(int count)
    {
        if (CurrentPassengers + count <= MaxPassengers && State == ElevatorState.DoorsOpen)
        {
            CurrentPassengers += count;
            ModifiedAt = DateTime.UtcNow;
            Version++;
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Removes passengers from the elevator
    /// </summary>
    /// <param name="count">Number of passengers to remove</param>
    /// <returns>True if passengers removed successfully, false if count exceeds current passengers</returns>
    public bool RemovePassengers(int count)
    {
        if (CurrentPassengers >= count && State == ElevatorState.DoorsOpen)
        {
            CurrentPassengers -= count;
            ModifiedAt = DateTime.UtcNow;
            Version++;
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Checks if the elevator can accept more passengers
    /// </summary>
    /// <returns>True if elevator can accept more passengers, false otherwise</returns>
    public bool CanAcceptPassengers()
    {
        return CurrentPassengers < MaxPassengers && State == ElevatorState.DoorsOpen;
    }
    
    /// <summary>
    /// Checks if the elevator is at a specific floor
    /// </summary>
    /// <param name="floorNumber">The floor number to check</param>
    /// <returns>True if elevator is at the specified floor, false otherwise</returns>
    public bool IsAtFloor(int floorNumber)
    {
        return CurrentFloor == floorNumber;
    }
    
    /// <summary>
    /// Checks if the elevator is heading in a specific direction
    /// </summary>
    /// <param name="direction">The direction to check</param>
    /// <returns>True if elevator is heading in the specified direction, false otherwise</returns>
    public bool IsHeadingInDirection(ElevatorDirection direction)
    {
        return Direction == direction || Direction == ElevatorDirection.None;
    }
    
    /// <summary>
    /// Gets the estimated time to reach a specific floor
    /// </summary>
    /// <param name="floorNumber">The target floor number</param>
    /// <returns>Estimated time in seconds</returns>
    public double GetEstimatedTimeToFloor(int floorNumber)
    {
        var distance = Math.Abs(floorNumber - CurrentFloor);
        var travelTime = distance / Speed;
        var doorTime = DoorOperationTime * 2; // Open and close
        
        return travelTime + doorTime;
    }
    
    /// <summary>
    /// Sets the elevator under maintenance
    /// </summary>
    public void SetUnderMaintenance()
    {
        IsUnderMaintenance = true;
        State = ElevatorState.UnderMaintenance;
        Direction = ElevatorDirection.None;
        DestinationFloors.Clear();
        LastMaintenanceDate = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        Version++;
    }
    
    /// <summary>
    /// Removes the elevator from maintenance
    /// </summary>
    public void RemoveFromMaintenance()
    {
        IsUnderMaintenance = false;
        State = ElevatorState.Idle;
        ModifiedAt = DateTime.UtcNow;
        Version++;
    }
    
    private void UpdateDirection()
    {
        if (DestinationFloors.Count == 0)
        {
            Direction = ElevatorDirection.None;
            State = ElevatorState.Idle;
        }
        else
        {
            var sortedDestinations = GetSortedDestinations();
            if (sortedDestinations.Count > 0)
            {
                var nextFloor = sortedDestinations[0];
                Direction = nextFloor > CurrentFloor ? ElevatorDirection.Up : ElevatorDirection.Down;
            }
        }
    }
    
    private List<int> GetSortedDestinations()
    {
        var destinations = new List<int>(DestinationFloors);
        
        if (Direction == ElevatorDirection.Up)
        {
            destinations.Sort(); // Ascending order for going up
        }
        else if (Direction == ElevatorDirection.Down)
        {
            destinations.Sort((a, b) => b.CompareTo(a)); // Descending order for going down
        }
        else
        {
            // If no direction, sort by distance from current floor
            destinations.Sort((a, b) => Math.Abs(a - CurrentFloor).CompareTo(Math.Abs(b - CurrentFloor)));
        }
        
        return destinations;
    }
}
