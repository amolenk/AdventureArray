using System.Diagnostics.CodeAnalysis;
using AdventureArray.Application.Shared.Messages;
using AdventureArray.Infrastructure.Features;
using AdventureArray.Infrastructure.Messaging.Configuration;
using AdventureArray.Infrastructure.Messaging.SerDes;
using MassTransit;
using Microsoft.Extensions.Options;

namespace AdventureArray.Application.RideService.Features.Rides.StoreWaitTimes;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class StoreWaitTimesFeatureSetup : FeatureSetup
{
	public override string FeatureFlagName => nameof(StoreWaitTimes);

	public override void RegisterDependencies(IServiceCollection services, IConfigurationManager configurationManager)
	{
		services.Configure<StoreWaitTimesSettings>(
			configurationManager.GetSection(StoreWaitTimesSettings.SectionName));
	}

	public override void ConfigureRider(IRiderRegistrationConfigurator configurator, IConfiguration configuration)
	{
		var settings = new StoreWaitTimesSettings();
		configuration.GetSection(StoreWaitTimesSettings.SectionName).Bind(settings);

		configurator.AddConsumer<StoreWaitTimesHandler>(c =>
		{
			c.Options<BatchOptions>(options => options
				.SetMessageLimit(settings.BatchMessageLimit)
				.SetTimeLimit(settings.BatchTimeLimit)
				.GroupBy<WaitTimeMessage, int>(x => x.Partition()));
		});
	}

	public override void ConfigureRiderEndpoints(IRiderRegistrationContext context,
		IKafkaFactoryConfigurator configurator)
	{
		var settings = context.GetRequiredService<IOptions<StoreWaitTimesSettings>>().Value;

		configurator.TopicEndpoint<WaitTimeMessage>(
			Shared.Constants.Messaging.Topics.WaitTimes,
			settings.ConsumerGroupId,
			e =>
			{
				e.ConfigureConsumer<StoreWaitTimesHandler>(context);
				e.SetValueDeserializer(new KafkaProtoBufDeserializer<WaitTimeMessage>());
				e.AutoOffsetReset = settings.AutoOffsetReset;
				e.PrefetchCount = settings.PrefetchCount;

				// Process multiple partitions in parallel.
				e.ConcurrentConsumerLimit = settings.ConcurrentConsumerLimit;
			});
	}
}
