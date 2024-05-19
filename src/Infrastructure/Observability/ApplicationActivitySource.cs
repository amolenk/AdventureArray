namespace AdventureArray.Infrastructure.Observability;

public static class ApplicationActivitySource
{
	public const string Name = "AdventureArray.Application";

	public static ActivitySource Source { get; } = new(Name);
}
