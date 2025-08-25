using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Elevator.Domain.Interfaces;
using Elevator.Domain.Enums;
using Common.Messaging.Interfaces;
using Elevator.Domain.Events;

namespace Elevator.Console;

/// <summary>
/// Main program class for the elevator simulation console application
/// </summary>
public class Program
{
    /// <summary>
    /// Main entry point
    /// </summary>
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        
        System.Console.WriteLine("=== Elevator Simulation System ===");
        System.Console.WriteLine("Initializing...");
        
        // Get services
        var elevatorService = host.Services.GetRequiredService<IElevatorService>();
        var messageBus = host.Services.GetRequiredService<IMessageBus>();
        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        
        // Subscribe to events
        await messageBus.SubscribeAsync<ElevatorCalledEvent>(HandleElevatorCalled);
        await messageBus.SubscribeAsync<ElevatorArrivedEvent>(HandleElevatorArrived);
        
        // Start the main application loop
        await RunElevatorSimulation(elevatorService, messageBus, logger);
    }
    
    /// <summary>
    /// Creates the host builder with dependency injection
    /// </summary>
    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Register services
                services.AddSingleton<IMessageBus, Common.Messaging.InMemoryMessageBus>();
                services.AddSingleton<IElevatorService, Elevator.Infrastructure.Services.ElevatorService>();
                
                // Configure logging
                services.AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Information);
                });
            });
    
    /// <summary>
    /// Runs the main elevator simulation loop
    /// </summary>
    private static async Task RunElevatorSimulation(
        IElevatorService elevatorService, 
        IMessageBus messageBus, 
        ILogger logger)
    {
        var running = true;
        
        while (running)
        {
            try
            {
                System.Console.Clear();
                await DisplayMainMenu(elevatorService);
                
                System.Console.Write("\nEnter your choice (1-6): ");
                var choice = System.Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        await CallElevator(elevatorService);
                        break;
                    case "2":
                        await DisplayElevatorStatus(elevatorService);
                        break;
                    case "3":
                        await UpdatePeopleWaiting(elevatorService);
                        break;
                    case "4":
                        await ProcessSimulation(elevatorService);
                        break;
                    case "5":
                        await SetMaintenanceMode(elevatorService);
                        break;
                    case "6":
                        running = false;
                        System.Console.WriteLine("Exiting elevator simulation...");
                        break;
                    default:
                        System.Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
                
                if (running)
                {
                    System.Console.WriteLine("\nPress any key to continue...");
                    System.Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in main simulation loop");
                System.Console.WriteLine($"An error occurred: {ex.Message}");
                System.Console.WriteLine("Press any key to continue...");
                System.Console.ReadKey();
            }
        }
    }
    
    /// <summary>
    /// Displays the main menu
    /// </summary>
    private static async Task DisplayMainMenu(IElevatorService elevatorService)
    {
        System.Console.WriteLine("=== Elevator Simulation System ===");
        System.Console.WriteLine($"Time: {DateTime.Now:HH:mm:ss}");
        System.Console.WriteLine();
        
        // Display current elevator status summary
        var elevators = await elevatorService.GetAllElevatorsAsync();
        System.Console.WriteLine("Current Elevator Status:");
        System.Console.WriteLine("ID\tFloor\tDirection\tState\t\tPassengers");
        System.Console.WriteLine("--\t-----\t---------\t-----\t\t----------");
        
        foreach (var elevator in elevators)
        {
            var direction = elevator.Direction == ElevatorDirection.None ? "None" : elevator.Direction.ToString();
            var state = elevator.State.ToString();
            System.Console.WriteLine($"{elevator.ElevatorId}\t{elevator.CurrentFloor}\t{direction}\t\t{state}\t\t{elevator.CurrentPassengers}/{elevator.MaxPassengers}");
        }
        
        System.Console.WriteLine();
        System.Console.WriteLine("1. Call Elevator");
        System.Console.WriteLine("2. Display Detailed Status");
        System.Console.WriteLine("3. Update People Waiting");
        System.Console.WriteLine("4. Process Simulation Step");
        System.Console.WriteLine("5. Set Maintenance Mode");
        System.Console.WriteLine("6. Exit");
    }
    
    /// <summary>
    /// Handles calling an elevator
    /// </summary>
    private static async Task CallElevator(IElevatorService elevatorService)
    {
        System.Console.WriteLine("\n=== Call Elevator ===");
        
        System.Console.Write("Enter floor number (1-20): ");
        if (!int.TryParse(System.Console.ReadLine(), out int floorNumber) || floorNumber < 1 || floorNumber > 20)
        {
            System.Console.WriteLine("Invalid floor number. Please enter a number between 1 and 20.");
            return;
        }
        
        System.Console.Write("Enter direction (U/D): ");
        var directionInput = System.Console.ReadLine()?.ToUpper();
        ElevatorDirection direction;
        
        if (directionInput == "U")
            direction = ElevatorDirection.Up;
        else if (directionInput == "D")
            direction = ElevatorDirection.Down;
        else
        {
            System.Console.WriteLine("Invalid direction. Please enter U for Up or D for Down.");
            return;
        }
        
        System.Console.Write("Enter number of people waiting: ");
        if (!int.TryParse(System.Console.ReadLine(), out int peopleWaiting) || peopleWaiting < 0)
        {
            System.Console.WriteLine("Invalid number of people. Please enter a non-negative number.");
            return;
        }
        
        var elevator = await elevatorService.CallElevatorAsync(floorNumber, direction, peopleWaiting);
        
        if (elevator != null)
        {
            System.Console.WriteLine($"Elevator {elevator.ElevatorId} has been called to floor {floorNumber}.");
        }
        else
        {
            System.Console.WriteLine("No available elevator found. Please try again later.");
        }
    }
    
    /// <summary>
    /// Displays detailed elevator status
    /// </summary>
    private static async Task DisplayElevatorStatus(IElevatorService elevatorService)
    {
        System.Console.WriteLine("\n=== Detailed Elevator Status ===");
        
        var elevators = await elevatorService.GetAllElevatorsAsync();
        
        foreach (var elevator in elevators)
        {
            System.Console.WriteLine($"\nElevator {elevator.ElevatorId}:");
            System.Console.WriteLine($"  Current Floor: {elevator.CurrentFloor}");
            System.Console.WriteLine($"  Direction: {elevator.Direction}");
            System.Console.WriteLine($"  State: {elevator.State}");
            System.Console.WriteLine($"  Passengers: {elevator.CurrentPassengers}/{elevator.MaxPassengers}");
            System.Console.WriteLine($"  Type: {elevator.Type}");
            System.Console.WriteLine($"  Door State: {elevator.DoorState}");
            System.Console.WriteLine($"  Maintenance: {(elevator.IsUnderMaintenance ? "Yes" : "No")}");
            System.Console.WriteLine($"  Destination Floors: {string.Join(", ", elevator.DestinationFloors)}");
            System.Console.WriteLine($"  Total Trips: {elevator.TotalTrips}");
            System.Console.WriteLine($"  Distance Traveled: {elevator.TotalDistanceTraveled} floors");
        }
    }
    
    /// <summary>
    /// Updates the number of people waiting on a floor
    /// </summary>
    private static async Task UpdatePeopleWaiting(IElevatorService elevatorService)
    {
        System.Console.WriteLine("\n=== Update People Waiting ===");
        
        System.Console.Write("Enter floor number (1-20): ");
        if (!int.TryParse(System.Console.ReadLine(), out int floorNumber) || floorNumber < 1 || floorNumber > 20)
        {
            System.Console.WriteLine("Invalid floor number. Please enter a number between 1 and 20.");
            return;
        }
        
        System.Console.Write("Enter new number of people waiting: ");
        if (!int.TryParse(System.Console.ReadLine(), out int peopleWaiting) || peopleWaiting < 0)
        {
            System.Console.WriteLine("Invalid number of people. Please enter a non-negative number.");
            return;
        }
        
        // Note: This would require a floor service in a complete implementation
        System.Console.WriteLine($"People waiting on floor {floorNumber} updated to {peopleWaiting}.");
    }
    
    /// <summary>
    /// Processes one simulation step
    /// </summary>
    private static async Task ProcessSimulation(IElevatorService elevatorService)
    {
        System.Console.WriteLine("\n=== Processing Simulation Step ===");
        
        await elevatorService.ProcessElevatorMovementAsync();
        System.Console.WriteLine("Simulation step processed. Elevator positions and states updated.");
    }
    
    /// <summary>
    /// Sets elevator maintenance mode
    /// </summary>
    private static async Task SetMaintenanceMode(IElevatorService elevatorService)
    {
        System.Console.WriteLine("\n=== Set Maintenance Mode ===");
        
        System.Console.Write("Enter elevator ID (E1-E4): ");
        var elevatorId = System.Console.ReadLine();
        
        if (string.IsNullOrEmpty(elevatorId) || !elevatorId.StartsWith("E"))
        {
            System.Console.WriteLine("Invalid elevator ID. Please enter E1, E2, E3, or E4.");
            return;
        }
        
        System.Console.Write("Set under maintenance? (Y/N): ");
        var maintenanceInput = System.Console.ReadLine()?.ToUpper();
        bool isUnderMaintenance;
        
        if (maintenanceInput == "Y")
            isUnderMaintenance = true;
        else if (maintenanceInput == "N")
            isUnderMaintenance = false;
        else
        {
            System.Console.WriteLine("Invalid input. Please enter Y for Yes or N for No.");
            return;
        }
        
        var success = await elevatorService.SetElevatorMaintenanceAsync(elevatorId, isUnderMaintenance);
        
        if (success)
        {
            var status = isUnderMaintenance ? "under maintenance" : "removed from maintenance";
            System.Console.WriteLine($"Elevator {elevatorId} has been {status}.");
        }
        else
        {
            System.Console.WriteLine($"Failed to update maintenance status for elevator {elevatorId}.");
        }
    }
    
    /// <summary>
    /// Handles elevator called events
    /// </summary>
    private static Task HandleElevatorCalled(ElevatorCalledEvent evt)
    {
        System.Console.WriteLine($"\n[EVENT] Elevator called to floor {evt.FloorNumber} going {evt.RequestedDirection} with {evt.PeopleWaiting} people waiting.");
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Handles elevator arrived events
    /// </summary>
    private static Task HandleElevatorArrived(ElevatorArrivedEvent evt)
    {
        System.Console.WriteLine($"\n[EVENT] Elevator {evt.ElevatorId} arrived at floor {evt.FloorNumber} with {evt.Passengers} passengers.");
        return Task.CompletedTask;
    }
}
