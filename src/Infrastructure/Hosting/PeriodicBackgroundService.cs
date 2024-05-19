namespace AdventureArray.Infrastructure.Hosting;

public abstract class PeriodicBackgroundService : ResilientBackgroundService
{
	private readonly TimeProvider _timeProvider;

	protected PeriodicBackgroundService(TimeProvider timeProvider, ILogger logger) :
		base(logger)
	{
		ArgumentNullException.ThrowIfNull(timeProvider);

		_timeProvider = timeProvider;
	}

	protected abstract TimeSpan Periode { get; }

	protected abstract Task WhenTimerFiresAsync(CancellationToken cancellationToken);

	protected override async Task ExecuteCoreAsync(CancellationToken stoppingToken)
	{
		using var timer = new PeriodicTimer(Periode, _timeProvider);

		do
		{
			await WhenTimerFiresAsync(stoppingToken);
		} while (await timer.WaitForNextTickAsync(stoppingToken));
	}
}
