namespace AdventureArray.Domain.Shared;

/// <summary>
/// Marker interface voor concurrent entities. De <see cref="Versie"/> property wordt gebruikt voor optimistic concurrency control en automatisch opgehoogd in <see cref="ApplicationDbContext.SaveChangesAsync"/>.
/// </summary>
public interface IConcurrentEntity
{
	/// <summary>
	/// Waarde voor concurrency control
	/// </summary>
	long Versie { get; set; }
}
