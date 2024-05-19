namespace AdventureArray.Application.UI.Client.Infrastructure.ErrorHandling;

/// <summary>
/// Provider for the unhandled exception logger.
/// </summary>
public sealed class UnhandledExceptionLoggerProvider : ILoggerProvider
{
	private readonly IServiceProvider _serviceProvider;

	public UnhandledExceptionLoggerProvider(IServiceProvider serviceProvider)
	{
		ArgumentNullException.ThrowIfNull(serviceProvider);

		_serviceProvider = serviceProvider;
	}

	public ILogger CreateLogger(string categoryName)
	{
		// We only want to log unhandled exceptions and don't need to filter by category.
		return new UnhandledExceptionLogger(_serviceProvider);
	}

	public void Dispose()
	{
	}
}
