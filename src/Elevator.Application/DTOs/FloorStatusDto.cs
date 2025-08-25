namespace Elevator.Application.DTOs;

/// <summary>
/// Data transfer object for floor status information
/// </summary>
public class FloorStatusDto
{
    /// <summary>
    /// Gets or sets the floor number
    /// </summary>
    public int FloorNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the number of people waiting
    /// </summary>
    public int PeopleWaiting { get; set; }
    
    /// <summary>
    /// Gets or sets whether the up button is pressed
    /// </summary>
    public bool UpButtonPressed { get; set; }
    
    /// <summary>
    /// Gets or sets whether the down button is pressed
    /// </summary>
    public bool DownButtonPressed { get; set; }
    
    /// <summary>
    /// Gets or sets the maximum capacity
    /// </summary>
    public int MaxCapacity { get; set; }
    
    /// <summary>
    /// Gets or sets whether the floor is accessible
    /// </summary>
    public bool IsAccessible { get; set; }
    
    /// <summary>
    /// Gets or sets the estimated wait time for an elevator
    /// </summary>
    public double EstimatedWaitTime { get; set; }
}
