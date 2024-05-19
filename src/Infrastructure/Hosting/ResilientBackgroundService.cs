namespace AdventureArray.Infrastructure.Hosting;

public abstract class ResilientBackgroundService : BackgroundService
{
	private readonly ILogger _logger;

	protected ResilientBackgroundService(ILogger logger)
	{
		ArgumentNullException.ThrowIfNull(logger);

		_logger = logger;
	}

	protected abstract TimeSpan TijdTussenHerstarten { get; }

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		// Gebruik Task.Run zodat de BackgroundService het opstarten van andere componenten niet blokkeert.
		await Task.Run(() => SafeExecuteAsync(stoppingToken), stoppingToken);
	}

	protected abstract Task ExecuteCoreAsync(CancellationToken stoppingToken);

	protected virtual async Task HandelFoutAfAsync(Exception fout, CancellationToken stoppingToken)
	{
		_logger.LogError(
			fout,
			"Er is een fout opgetreden. Service wordt herstart over {TijdTussenHerstarten}.",
			TijdTussenHerstarten);

		try
		{
			await Task.Delay(TijdTussenHerstarten, stoppingToken);
		}
		catch (OperationCanceledException)
		{
			// Negeer
		}
	}

	private async Task SafeExecuteAsync(CancellationToken stoppingToken)
	{
		// De service moet blijven werken totdat de token wordt geannuleerd.
		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await ExecuteCoreAsync(stoppingToken);
			}
			catch (OperationCanceledException)
			{
				// Negeer
			}
			catch (Exception e)
			{
				try
				{
					await HandelFoutAfAsync(e, stoppingToken);
				}
				catch (Exception handelFoutAfException)
				{
					// De service mag niet crashen als het afhandelen van een fout mislukt.
					_logger.LogError(
						handelFoutAfException,
						"Er is een fout opgetreden tijdens het afhandelen van een fout.");
				}
			}
		}
	}
}
