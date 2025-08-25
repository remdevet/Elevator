using Common.Dry.Base;

namespace Elevator.Domain.Entities;

/// <summary>
/// Represents a floor in the building
/// </summary>
public class Floor : EntityBase
{
    /// <summary>
    /// Gets or sets the floor number
    /// </summary>
    public int FloorNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the number of people waiting on this floor
    /// </summary>
    public int PeopleWaiting { get; set; }
    
    /// <summary>
    /// Gets or sets whether there's an elevator call button pressed for going up
    /// </summary>
    public bool UpButtonPressed { get; set; }
    
    /// <summary>
    /// Gets or sets whether there's an elevator call button pressed for going down
    /// </summary>
    public bool DownButtonPressed { get; set; }
    
    /// <summary>
    /// Gets or sets the maximum capacity of people that can wait on this floor
    /// </summary>
    public int MaxCapacity { get; set; }
    
    /// <summary>
    /// Gets or sets whether this floor is accessible (e.g., not a basement or restricted floor)
    /// </summary>
    public bool IsAccessible { get; set; }
    
    public Floor(int floorNumber, int maxCapacity = 100, bool isAccessible = true)
    {
        FloorNumber = floorNumber;
        MaxCapacity = maxCapacity;
        IsAccessible = isAccessible;
        PeopleWaiting = 0;
        UpButtonPressed = false;
        DownButtonPressed = false;
    }
    
    /// <summary>
    /// Adds people waiting on this floor
    /// </summary>
    /// <param name="count">Number of people to add</param>
    /// <returns>True if successful, false if capacity exceeded</returns>
    public bool AddPeopleWaiting(int count)
    {
        if (PeopleWaiting + count <= MaxCapacity)
        {
            PeopleWaiting += count;
            ModifiedAt = DateTime.UtcNow;
            Version++;
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Removes people waiting on this floor
    /// </summary>
    /// <param name="count">Number of people to remove</param>
    /// <returns>True if successful, false if count exceeds people waiting</returns>
    public bool RemovePeopleWaiting(int count)
    {
        if (PeopleWaiting >= count)
        {
            PeopleWaiting -= count;
            ModifiedAt = DateTime.UtcNow;
            Version++;
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Presses the up button
    /// </summary>
    public void PressUpButton()
    {
        UpButtonPressed = true;
        ModifiedAt = DateTime.UtcNow;
        Version++;
    }
    
    /// <summary>
    /// Presses the down button
    /// </summary>
    public void PressDownButton()
    {
        DownButtonPressed = true;
        ModifiedAt = DateTime.UtcNow;
        Version++;
    }
    
    /// <summary>
    /// Releases the up button
    /// </summary>
    public void ReleaseUpButton()
    {
        UpButtonPressed = false;
        ModifiedAt = DateTime.UtcNow;
        Version++;
    }
    
    /// <summary>
    /// Releases the down button
    /// </summary>
    public void ReleaseDownButton()
    {
        DownButtonPressed = false;
        ModifiedAt = DateTime.UtcNow;
        Version++;
    }
}
