namespace AdventureArray.Application.UI.Client.Infrastructure.ErrorHandling;

/// <summary>
/// Blazor sends unhandled exceptions to all loggers. By creating a custom logger,
/// we can intercept these exceptions and send them to the Error component for display.
/// </summary>
public class UnhandledExceptionLogger : ILogger
{
	private readonly IServiceProvider _serviceProvider;

	public UnhandledExceptionLogger(IServiceProvider serviceProvider)
	{
		ArgumentNullException.ThrowIfNull(serviceProvider);

		_serviceProvider = serviceProvider;
	}

	public bool IsEnabled(LogLevel logLevel) => true;

	public void Log<TState>(
		LogLevel logLevel,
		EventId eventId,
		TState state,
		Exception? exception,
		Func<TState, Exception, string> formatter)
	{
		if (exception is null) return;

		// Use the service provider to get the mediator and send the exception to the error handler.
		// We do not inject the mediator directly because it would create a runtime circular dependency
		// with the logger provider, which causes the UI not to load.
		var mediator = _serviceProvider.GetRequiredService<IMediator>();
		_ = mediator.Send(new ErrorHandler.HandleExceptionCommand(exception));
	}

	public IDisposable BeginScope<TState>(TState state) where TState : notnull
	{
		return new NoopDisposable();
	}

	private sealed class NoopDisposable : IDisposable
	{
		public void Dispose()
		{
		}
	}
}
