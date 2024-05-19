namespace AdventureArray.Infrastructure.Persistence.Customization;

/// <summary>
/// This class contains constant values for Citus database annotations.
/// </summary>
public static class CustomDataAnnotations
{
	/// <summary>
	/// Annotation used to specify that the Citus extension must be installed.
	/// </summary>
	public const string EnsureCitusExtension = "EnsureCitusExtension";

	/// <summary>
	/// Annotation used to specify the column for range partitioning in Citus.
	/// </summary>
	public const string PartitionByRange = "PartitionByRange";

	/// <summary>
	/// Annotation used to create a Citus distributed table.
	/// </summary>
	public const string Distribute = "Distribute";

	/// <summary>
	/// Annotation used to colocate distributed table with another distributed table. The distributed column name must be the same for both tables.
	/// </summary>
	public const string Colocated = "Colocated";

	/// <summary>
	/// Annotation used to create a Citus reference table.
	/// </summary>
	public const string Reference = "Reference";
}
