namespace AdventureArray.Domain.Shared;

/// <summary>
/// Marker interface voor entities met een AangemaaktOp tijdstip. <see cref="AangemaaktOp"/> krijgt automatisch een waarde in <see cref="ApplicationDbContext.SaveChangesAsync"/>.
/// </summary>
public interface IHasCreationTimestamp
{
	/// <summary>
	/// Tijdstip waarop de entiteit is aangemaakt.
	/// </summary>
	DateTime AangemaaktOp { get; set; }
}
