using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging;
using Common.Messaging.Interfaces;
using Elevator.Domain.Entities;
using Elevator.Domain.Enums;
using Elevator.Domain.Events;
using Elevator.Infrastructure.Services;

namespace Elevator.Tests;

/// <summary>
/// Unit tests for the elevator system
/// </summary>
public class ElevatorTests
{
    private readonly Mock<ILogger<ElevatorService>> _mockLogger;
    private readonly Mock<IMessageBus> _mockMessageBus;
    private readonly ElevatorService _elevatorService;
    
    public ElevatorTests()
    {
        _mockLogger = new Mock<ILogger<ElevatorService>>();
        _mockMessageBus = new Mock<IMessageBus>();
        _elevatorService = new ElevatorService(_mockLogger.Object, _mockMessageBus.Object);
    }
    
    [Fact]
    public async Task CallElevator_WithValidParameters_ShouldReturnElevator()
    {
        // Arrange
        var floorNumber = 5;
        var direction = ElevatorDirection.Up;
        var peopleWaiting = 3;
        
        _mockMessageBus.Setup(x => x.PublishAsync(It.IsAny<ElevatorCalledEvent>()))
            .Returns(Task.CompletedTask);
        
        // Act
        var result = await _elevatorService.CallElevatorAsync(floorNumber, direction, peopleWaiting);
        
        // Assert
        result.Should().NotBeNull();
        result!.ElevatorId.Should().NotBeNullOrEmpty();
        result.DestinationFloors.Should().Contain(floorNumber);
        
        _mockMessageBus.Verify(x => x.PublishAsync(It.IsAny<ElevatorCalledEvent>()), Times.Once);
    }
    
    [Fact]
    public async Task CallElevator_WithInvalidFloorNumber_ShouldReturnNull()
    {
        // Arrange
        var floorNumber = 25; // Invalid floor
        var direction = ElevatorDirection.Up;
        var peopleWaiting = 3;
        
        // Act
        var result = await _elevatorService.CallElevatorAsync(floorNumber, direction, peopleWaiting);
        
        // Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task GetNearestAvailableElevator_ShouldReturnBestElevator()
    {
        // Arrange
        var floorNumber = 10;
        var direction = ElevatorDirection.Up;
        
        // Act
        var result = await _elevatorService.GetNearestAvailableElevatorAsync(floorNumber, direction);
        
        // Assert
        result.Should().NotBeNull();
        result!.IsUnderMaintenance.Should().BeFalse();
        result.State.Should().NotBe(ElevatorState.OutOfService);
    }
    
    [Fact]
    public async Task GetAllElevators_ShouldReturnAllElevators()
    {
        // Act
        var result = await _elevatorService.GetAllElevatorsAsync();
        
        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(4); // We initialize 4 elevators
        result.Should().Contain(e => e.ElevatorId == "E1");
        result.Should().Contain(e => e.ElevatorId == "E2");
        result.Should().Contain(e => e.ElevatorId == "E3");
        result.Should().Contain(e => e.ElevatorId == "E4");
    }
    
    [Fact]
    public async Task GetElevatorStatus_WithValidId_ShouldReturnElevator()
    {
        // Arrange
        var elevatorId = "E1";
        
        // Act
        var result = await _elevatorService.GetElevatorStatusAsync(elevatorId);
        
        // Assert
        result.Should().NotBeNull();
        result!.ElevatorId.Should().Be(elevatorId);
    }
    
    [Fact]
    public async Task GetElevatorStatus_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var elevatorId = "INVALID";
        
        // Act
        var result = await _elevatorService.GetElevatorStatusAsync(elevatorId);
        
        // Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task SetElevatorMaintenance_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var elevatorId = "E1";
        var isUnderMaintenance = true;
        
        // Act
        var result = await _elevatorService.SetElevatorMaintenanceAsync(elevatorId, isUnderMaintenance);
        
        // Assert
        result.Should().BeTrue();
        
        var elevator = await _elevatorService.GetElevatorStatusAsync(elevatorId);
        elevator!.IsUnderMaintenance.Should().BeTrue();
        elevator.State.Should().Be(ElevatorState.UnderMaintenance);
    }
    
    [Fact]
    public async Task SetElevatorMaintenance_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var elevatorId = "INVALID";
        var isUnderMaintenance = true;
        
        // Act
        var result = await _elevatorService.SetElevatorMaintenanceAsync(elevatorId, isUnderMaintenance);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public async Task ProcessElevatorMovement_ShouldUpdateElevatorStates()
    {
        // Arrange
        var elevator = await _elevatorService.GetElevatorStatusAsync("E1");
        elevator!.AddDestinationFloor(5);
        
        // Act - Process movement multiple times to simulate elevator travel
        await _elevatorService.ProcessElevatorMovementAsync(); // First call should start movement
        await _elevatorService.ProcessElevatorMovementAsync(); // Second call should continue movement
        await _elevatorService.ProcessElevatorMovementAsync(); // Third call should continue movement
        await _elevatorService.ProcessElevatorMovementAsync(); // Fourth call should arrive at floor 5
        
        // Assert
        var updatedElevator = await _elevatorService.GetElevatorStatusAsync("E1");
        updatedElevator!.CurrentFloor.Should().Be(5);
        updatedElevator.DestinationFloors.Should().NotContain(5);
    }
}

/// <summary>
/// Unit tests for the Elevator entity
/// </summary>
public class ElevatorEntityTests
{
    [Fact]
    public void Elevator_Constructor_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var elevator = new Elevator.Domain.Entities.Elevator("TEST", 10, ElevatorType.Standard, 1.5, 2.0);
        
        // Assert
        elevator.ElevatorId.Should().Be("TEST");
        elevator.CurrentFloor.Should().Be(1);
        elevator.Direction.Should().Be(ElevatorDirection.None);
        elevator.State.Should().Be(ElevatorState.Idle);
        elevator.CurrentPassengers.Should().Be(0);
        elevator.MaxPassengers.Should().Be(10);
        elevator.Type.Should().Be(ElevatorType.Standard);
        elevator.Speed.Should().Be(1.5);
        elevator.DoorOperationTime.Should().Be(2.0);
        elevator.DoorState.Should().Be(DoorState.Closed);
        elevator.IsUnderMaintenance.Should().BeFalse();
        elevator.TotalTrips.Should().Be(0);
        elevator.TotalDistanceTraveled.Should().Be(0);
    }
    
    [Fact]
    public void AddDestinationFloor_WithNewFloor_ShouldAddSuccessfully()
    {
        // Arrange
        var elevator = new Elevator.Domain.Entities.Elevator("TEST", 10);
        
        // Act
        var result = elevator.AddDestinationFloor(5);
        
        // Assert
        result.Should().BeTrue();
        elevator.DestinationFloors.Should().Contain(5);
        elevator.Direction.Should().Be(ElevatorDirection.Up);
        elevator.State.Should().Be(ElevatorState.Idle);
    }
    
    [Fact]
    public void AddDestinationFloor_WithExistingFloor_ShouldReturnFalse()
    {
        // Arrange
        var elevator = new Elevator.Domain.Entities.Elevator("TEST", 10);
        elevator.AddDestinationFloor(5);
        
        // Act
        var result = elevator.AddDestinationFloor(5);
        
        // Assert
        result.Should().BeFalse();
        elevator.DestinationFloors.Should().HaveCount(1);
    }
    
    [Fact]
    public void MoveToNextDestination_WithValidDestination_ShouldMoveSuccessfully()
    {
        // Arrange
        var elevator = new Elevator.Domain.Entities.Elevator("TEST", 10);
        elevator.AddDestinationFloor(5);
        
        // Act
        var result = elevator.MoveToNextDestination();
        
        // Assert
        result.Should().BeTrue();
        elevator.CurrentFloor.Should().Be(5);
        elevator.State.Should().Be(ElevatorState.Idle);
        elevator.DestinationFloors.Should().NotContain(5);
        elevator.TotalTrips.Should().Be(1);
        elevator.TotalDistanceTraveled.Should().Be(4);
    }
    
    [Fact]
    public void MoveToNextDestination_WithNoDestinations_ShouldReturnFalse()
    {
        // Arrange
        var elevator = new Elevator.Domain.Entities.Elevator("TEST", 10);
        
        // Act
        var result = elevator.MoveToNextDestination();
        
        // Assert
        result.Should().BeFalse();
        elevator.CurrentFloor.Should().Be(1);
        elevator.State.Should().Be(ElevatorState.Idle);
    }
    
    [Fact]
    public void OpenDoors_WhenIdle_ShouldOpenSuccessfully()
    {
        // Arrange
        var elevator = new Elevator.Domain.Entities.Elevator("TEST", 10);
        
        // Act
        var result = elevator.OpenDoors();
        
        // Assert
        result.Should().BeTrue();
        elevator.DoorState.Should().Be(DoorState.Open);
        elevator.State.Should().Be(ElevatorState.DoorsOpen);
    }
    
    [Fact]
    public void CloseDoors_WhenOpen_ShouldCloseSuccessfully()
    {
        // Arrange
        var elevator = new Elevator.Domain.Entities.Elevator("TEST", 10);
        elevator.OpenDoors();
        
        // Act
        var result = elevator.CloseDoors();
        
        // Assert
        result.Should().BeTrue();
        elevator.DoorState.Should().Be(DoorState.Closed);
        elevator.State.Should().Be(ElevatorState.Idle);
    }
    
    [Fact]
    public void AddPassengers_WhenDoorsOpen_ShouldAddSuccessfully()
    {
        // Arrange
        var elevator = new Elevator.Domain.Entities.Elevator("TEST", 10);
        elevator.OpenDoors();
        
        // Act
        var result = elevator.AddPassengers(3);
        
        // Assert
        result.Should().BeTrue();
        elevator.CurrentPassengers.Should().Be(3);
    }
    
    [Fact]
    public void AddPassengers_WhenDoorsClosed_ShouldReturnFalse()
    {
        // Arrange
        var elevator = new Elevator.Domain.Entities.Elevator("TEST", 10);
        
        // Act
        var result = elevator.AddPassengers(3);
        
        // Assert
        result.Should().BeFalse();
        elevator.CurrentPassengers.Should().Be(0);
    }
    
    [Fact]
    public void AddPassengers_WhenCapacityExceeded_ShouldReturnFalse()
    {
        // Arrange
        var elevator = new Elevator.Domain.Entities.Elevator("TEST", 5);
        elevator.OpenDoors();
        elevator.AddPassengers(5);
        
        // Act
        var result = elevator.AddPassengers(1);
        
        // Assert
        result.Should().BeFalse();
        elevator.CurrentPassengers.Should().Be(5);
    }
    
    [Fact]
    public void CanAcceptPassengers_WhenAvailable_ShouldReturnTrue()
    {
        // Arrange
        var elevator = new Elevator.Domain.Entities.Elevator("TEST", 10);
        elevator.OpenDoors();
        
        // Act
        var result = elevator.CanAcceptPassengers();
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void CanAcceptPassengers_WhenFull_ShouldReturnFalse()
    {
        // Arrange
        var elevator = new Elevator.Domain.Entities.Elevator("TEST", 5);
        elevator.OpenDoors();
        elevator.AddPassengers(5);
        
        // Act
        var result = elevator.CanAcceptPassengers();
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void GetEstimatedTimeToFloor_ShouldCalculateCorrectly()
    {
        // Arrange
        var elevator = new Elevator.Domain.Entities.Elevator("TEST", 10, ElevatorType.Standard, 2.0, 3.0);
        elevator.AddDestinationFloor(5);
        
        // Act
        var estimatedTime = elevator.GetEstimatedTimeToFloor(5);
        
        // Assert
        estimatedTime.Should().Be(8.0); // 4 floors / 2.0 speed + 6 seconds for doors
    }
    
    [Fact]
    public void SetUnderMaintenance_ShouldSetCorrectState()
    {
        // Arrange
        var elevator = new Elevator.Domain.Entities.Elevator("TEST", 10);
        
        // Act
        elevator.SetUnderMaintenance();
        
        // Assert
        elevator.IsUnderMaintenance.Should().BeTrue();
        elevator.State.Should().Be(ElevatorState.UnderMaintenance);
        elevator.Direction.Should().Be(ElevatorDirection.None);
        elevator.DestinationFloors.Should().BeEmpty();
        elevator.LastMaintenanceDate.Should().NotBeNull();
    }
    
    [Fact]
    public void RemoveFromMaintenance_ShouldRestoreNormalState()
    {
        // Arrange
        var elevator = new Elevator.Domain.Entities.Elevator("TEST", 10);
        elevator.SetUnderMaintenance();
        
        // Act
        elevator.RemoveFromMaintenance();
        
        // Assert
        elevator.IsUnderMaintenance.Should().BeFalse();
        elevator.State.Should().Be(ElevatorState.Idle);
    }
}

/// <summary>
/// Unit tests for the Floor entity
/// </summary>
public class FloorEntityTests
{
    [Fact]
    public void Floor_Constructor_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var floor = new Floor(5, 150, true);
        
        // Assert
        floor.FloorNumber.Should().Be(5);
        floor.MaxCapacity.Should().Be(150);
        floor.IsAccessible.Should().BeTrue();
        floor.PeopleWaiting.Should().Be(0);
        floor.UpButtonPressed.Should().BeFalse();
        floor.DownButtonPressed.Should().BeFalse();
    }
    
    [Fact]
    public void AddPeopleWaiting_WithinCapacity_ShouldAddSuccessfully()
    {
        // Arrange
        var floor = new Floor(5, 100);
        
        // Act
        var result = floor.AddPeopleWaiting(30);
        
        // Assert
        result.Should().BeTrue();
        floor.PeopleWaiting.Should().Be(30);
    }
    
    [Fact]
    public void AddPeopleWaiting_ExceedingCapacity_ShouldReturnFalse()
    {
        // Arrange
        var floor = new Floor(5, 100);
        
        // Act
        var result = floor.AddPeopleWaiting(150);
        
        // Assert
        result.Should().BeFalse();
        floor.PeopleWaiting.Should().Be(0);
    }
    
    [Fact]
    public void RemovePeopleWaiting_ValidCount_ShouldRemoveSuccessfully()
    {
        // Arrange
        var floor = new Floor(5, 100);
        floor.AddPeopleWaiting(50);
        
        // Act
        var result = floor.RemovePeopleWaiting(20);
        
        // Assert
        result.Should().BeTrue();
        floor.PeopleWaiting.Should().Be(30);
    }
    
    [Fact]
    public void RemovePeopleWaiting_ExceedingCurrent_ShouldReturnFalse()
    {
        // Arrange
        var floor = new Floor(5, 100);
        floor.AddPeopleWaiting(30);
        
        // Act
        var result = floor.RemovePeopleWaiting(50);
        
        // Assert
        result.Should().BeFalse();
        floor.PeopleWaiting.Should().Be(30);
    }
    
    [Fact]
    public void PressUpButton_ShouldSetButtonPressed()
    {
        // Arrange
        var floor = new Floor(5, 100);
        
        // Act
        floor.PressUpButton();
        
        // Assert
        floor.UpButtonPressed.Should().BeTrue();
    }
    
    [Fact]
    public void PressDownButton_ShouldSetButtonPressed()
    {
        // Arrange
        var floor = new Floor(5, 100);
        
        // Act
        floor.PressDownButton();
        
        // Assert
        floor.DownButtonPressed.Should().BeTrue();
    }
    
    [Fact]
    public void ReleaseUpButton_ShouldClearButtonPressed()
    {
        // Arrange
        var floor = new Floor(5, 100);
        floor.PressUpButton();
        
        // Act
        floor.ReleaseUpButton();
        
        // Assert
        floor.UpButtonPressed.Should().BeFalse();
    }
    
    [Fact]
    public void ReleaseDownButton_ShouldClearButtonPressed()
    {
        // Arrange
        var floor = new Floor(5, 100);
        floor.PressDownButton();
        
        // Act
        floor.ReleaseDownButton();
        
        // Assert
        floor.DownButtonPressed.Should().BeFalse();
    }
}
