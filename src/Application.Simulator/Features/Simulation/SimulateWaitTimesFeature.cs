using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using AdventureArray.Application.Shared.Messages;
using AdventureArray.Infrastructure.Persistence;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.EntityFrameworkCore;

namespace AdventureArray.Application.Simulator.Features.Simulation;

public class SimulateWaitTimesWorker : BackgroundService
{
	private readonly IMediator _mediator;
	private readonly ILogger _logger;

	public SimulateWaitTimesWorker(IMediator mediator, ILogger<SimulateWaitTimesWorker> logger)
	{
		ArgumentNullException.ThrowIfNull(mediator);
		ArgumentNullException.ThrowIfNull(logger);

		_mediator = mediator;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		using PeriodicTimer timer = new(TimeSpan.FromSeconds(5));

		_logger.LogInformation("Wait times simulation worker is starting.");

		try
		{
			while (await timer.WaitForNextTickAsync(stoppingToken))
			{
				await _mediator.Send(new SimulateWaitTimesCommand(), stoppingToken);
			}
		}
		catch (OperationCanceledException)
		{
			_logger.LogInformation("Wait times simulation worker is stopping.");
		}
	}
}

public record SimulateWaitTimesCommand;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class SimulateWaitTimesHandler : IConsumer<SimulateWaitTimesCommand>
{
	private readonly ApplicationDbContext _dbContext;
	private readonly ITopicProducer<string, WaitTimeMessage> _topicProducer;
	private readonly ILogger _logger;

	public SimulateWaitTimesHandler(ApplicationDbContext dbContext,
		ITopicProducer<string, WaitTimeMessage> topicProducer, ILogger<SimulateWaitTimesWorker> logger)
	{
		ArgumentNullException.ThrowIfNull(dbContext);
		ArgumentNullException.ThrowIfNull(topicProducer);
		ArgumentNullException.ThrowIfNull(logger);

		_dbContext = dbContext;
		_topicProducer = topicProducer;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<SimulateWaitTimesCommand> context)
	{
		var rides = await _dbContext.Rides.AsNoTracking().ToListAsync();
		var random = new Random();
		var count = 0;

		foreach (var ride in rides)
		{
			var key = ride.Id.ToString(CultureInfo.InvariantCulture);

			await _topicProducer.Produce(key, new WaitTimeMessage
			{
				RideId = ride.Id,
				WaitTimeMinutes = random.Next(1, 60),
				Timestamp = DateTime.UtcNow
			}, context.CancellationToken);

			count++;
		}

		_logger.LogInformation("Sent wait time updates for {Count} rides.", count);
	}
}


