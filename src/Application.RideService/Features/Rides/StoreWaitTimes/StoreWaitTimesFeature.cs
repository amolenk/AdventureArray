using System.Diagnostics.CodeAnalysis;
using AdventureArray.Application.Shared.Messages;
using AdventureArray.Infrastructure.Persistence;
using Confluent.Kafka;
using MassTransit;

namespace AdventureArray.Application.RideService.Features.Rides.StoreWaitTimes;

public class StoreWaitTimesSettings
{
	public const string SectionName = $"Features:{nameof(StoreWaitTimes)}";

	public string ConsumerGroupId { get; init; } = "ride-service";
	public AutoOffsetReset AutoOffsetReset { get; init; } = AutoOffsetReset.Earliest;
	public int BatchMessageLimit { get; init; } = 10;
	public TimeSpan BatchTimeLimit { get; init; } = TimeSpan.FromSeconds(1);
	public int PrefetchCount { get; init; } = 1000;
	public ushort ConcurrentConsumerLimit { get; init; } = 1;
}

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class StoreWaitTimesHandler : IConsumer<Batch<WaitTimeMessage>>
{
	private readonly ApplicationDbContext _dbContext;
	private readonly ILogger _logger;

	public StoreWaitTimesHandler(ApplicationDbContext dbContext, ILogger<StoreWaitTimesHandler> logger)
	{
		ArgumentNullException.ThrowIfNull(dbContext);
		ArgumentNullException.ThrowIfNull(logger);

		_dbContext = dbContext;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<Batch<WaitTimeMessage>> context)
	{
		foreach (var item in context.Message)
		{
			var ride = await _dbContext.Rides.FindAsync(item.Message.RideId);
			if (ride is null) continue;

			ride.WaitTimeMinutes = item.Message.WaitTimeMinutes;
			ride.WaitTimeUpdated = DateTime.SpecifyKind(item.Message.Timestamp, DateTimeKind.Utc);
		}

		await _dbContext.SaveChangesAsync();

		_logger.LogInformation("Stored wait times for {Count} rides.", context.Message.Length);
	}
}
