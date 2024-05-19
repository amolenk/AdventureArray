using System.Diagnostics.CodeAnalysis;
using AdventureArray.Application.Shared.Messages;
using AdventureArray.Infrastructure.Features;
using AdventureArray.Infrastructure.Messaging.Extensions;
using AdventureArray.Infrastructure.Messaging.SerDes;
using MassTransit;

namespace AdventureArray.Application.Simulator.Features.Simulation;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class SimulateWaitTimesFeatureSetup : FeatureSetup
{
	public override void RegisterDependencies(IServiceCollection services, IConfigurationManager configurationManager)
	{
		services.AddHostedService<SimulateWaitTimesWorker>();
	}

	public override void ConfigureMediator(IMediatorRegistrationConfigurator configurator)
	{
		configurator.AddConsumer<SimulateWaitTimesHandler>();
	}

	public override void ConfigureRider(IRiderRegistrationConfigurator configurator, IConfiguration configuration)
	{
		configurator.AddProducerIfNotExists<string, WaitTimeMessage>(
			Shared.Constants.Messaging.Topics.WaitTimes,
			(_, kafka) =>
			{
				kafka.SetValueSerializer(new KafkaProtoBufSerializer<WaitTimeMessage>());
			});
	}
}
