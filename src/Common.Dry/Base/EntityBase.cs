namespace Common.Dry.Base;

/// <summary>
/// Base entity class with common properties
/// </summary>
public abstract class EntityBase
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the last modification timestamp
    /// </summary>
    public DateTime? ModifiedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the version for optimistic concurrency
    /// </summary>
    public int Version { get; set; }
    
    protected EntityBase()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        Version = 1;
    }
}
