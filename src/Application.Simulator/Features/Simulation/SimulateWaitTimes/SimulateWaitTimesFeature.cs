using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using System.Globalization;
using AdventureArray.Application.Shared.Messages;
using AdventureArray.Infrastructure.Observability;
using AdventureArray.Infrastructure.Persistence;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.EntityFrameworkCore;
using Dapr.Client;

namespace AdventureArray.Application.Simulator.Features.Simulation.SimulateWaitTimes;

public sealed class SimulateWaitTimesWorker : ISimulatorService, IDisposable
{
	private readonly IMediator _mediator;
	private readonly ILogger _logger;
	private Timer? _timer;

	public SimulateWaitTimesWorker(IMediator mediator, ILogger<SimulateWaitTimesWorker> logger)
	{
		ArgumentNullException.ThrowIfNull(mediator);
		ArgumentNullException.ThrowIfNull(logger);

		_mediator = mediator;
		_logger = logger;
	}

	public bool IsRunning { get; private set; }

	public void Dispose()
	{
		_timer?.Dispose();
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Wait times simulation worker is starting.");

		IsRunning = true;

		_timer = new Timer(DoWork, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Wait times simulation worker is stopping.");
		IsRunning = false;

		_timer?.Dispose();

		return Task.CompletedTask;
	}

	private void DoWork(object? state)
	{
		_ = Task.Run(async () =>
		{
			var traceId = ActivityTraceId.CreateRandom();
			var spanId = ActivitySpanId.CreateRandom();
			var activityContext = new ActivityContext(traceId, spanId, ActivityTraceFlags.None);

			using var activity = ApplicationActivitySource.Source.StartActivity("timer fire",
				ActivityKind.Internal, activityContext);

			await _mediator.Send(new SimulateWaitTimesCommand());
		});
	}
}

public record SimulateWaitTimesCommand;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class SimulateWaitTimesHandler : IConsumer<SimulateWaitTimesCommand>
{
	private readonly ApplicationDbContext _dbContext;
	private readonly ITopicProducer<string, WaitTimeMessage> _topicProducer;
	private readonly SimulateWaitTimesMetrics _metrics;
	private readonly ILogger _logger;

	public SimulateWaitTimesHandler(ApplicationDbContext dbContext,
		ITopicProducer<string, WaitTimeMessage> topicProducer, SimulateWaitTimesMetrics metrics,
		ILogger<SimulateWaitTimesWorker> logger)
	{
		ArgumentNullException.ThrowIfNull(dbContext);
		ArgumentNullException.ThrowIfNull(topicProducer);
		ArgumentNullException.ThrowIfNull(metrics);
		ArgumentNullException.ThrowIfNull(logger);

		_dbContext = dbContext;
		_topicProducer = topicProducer;
		_metrics = metrics;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<SimulateWaitTimesCommand> context)
	{
		var rideIds = await GetRideIdsAsync();
		var random = new Random();
		var count = 0;

		foreach (var rideId in rideIds)
		{
			var key = rideId.ToString(CultureInfo.InvariantCulture);

			await _topicProducer.Produce(key, new WaitTimeMessage
			{
				RideId = rideId,
				WaitTimeMinutes = random.Next(1, 60),
				Timestamp = DateTime.UtcNow
			}, context.CancellationToken);

			count++;
		}

		_logger.LogInformation("Sent wait time updates for {Count} rides.", count);
		_metrics.WaitTimesGenerated.Add(count);
	}

	private async Task<int[]> GetRideIdsAsync()
	{
		return await _dbContext.Rides.AsNoTracking().Select(r => r.Id).ToArrayAsync();
	}
}

public sealed class SimulateWaitTimesMetrics
{
	public const string MeterName = "Simulator";

	// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
	private readonly Meter _meter;

    public Counter<long> WaitTimesGenerated { get; }

    public SimulateWaitTimesMetrics(IMeterFactory meterFactory)
    {
        _meter = meterFactory.Create(MeterName);

        WaitTimesGenerated = _meter.CreateCounter<long>(Counters.WaitTimesGenerated, Descriptions.WaitTimesGenerated);
    }

    private static class Counters
    {
        public const string WaitTimesGenerated = "simulator.wait_times.generated";
    }

    private static class Descriptions
    {
        public const string WaitTimesGenerated = "Total number of wait times generated";
    }
}

