namespace AdventureArray.Infrastructure.Messaging.Topology;

/// <summary>
/// Custom formatter for entity (topic/exchange) names.
/// </summary>
public sealed class CustomEntityNameFormatter : IEntityNameFormatter
{
	public string FormatEntityName<T>()
	{
		var entityName = KebabCaseEndpointNameFormatter.Instance.SanitizeName(typeof(T).FullName!);

		const string adventureArray = "adventurearray.";
		if (entityName.StartsWith(adventureArray, StringComparison.InvariantCulture))
		{
			entityName = entityName[adventureArray.Length..];
		}

		return entityName;
	}
}
