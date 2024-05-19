namespace AdventureArray.Infrastructure.Messaging.Topology;

/// <summary>
/// Custom formatter for endpoint names.
/// </summary>
public class CustomEndpointNameFormatter : KebabCaseEndpointNameFormatter
{
	public CustomEndpointNameFormatter(string prefix)
		: base(prefix)
	{
	}

	protected override string GetConsumerName(Type type)
	{
		var name = type.Name;

		const string handlerPostfix = "Handler";
		if (name.EndsWith(handlerPostfix, StringComparison.InvariantCulture))
		{
			name = name[..^handlerPostfix.Length];
		}

		return $"{Prefix}-{SanitizeName(name)}";
	}
}
