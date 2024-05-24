using System.Diagnostics.CodeAnalysis;
using AdventureArray.Application.Shared.Messages;
using AdventureArray.Infrastructure.Features;
using AdventureArray.Infrastructure.Messaging.Extensions;
using AdventureArray.Infrastructure.Messaging.SerDes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AdventureArray.Application.Simulator.Features.Simulation.SimulateWaitTimes;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class SimulateWaitTimesFeatureSetup : FeatureSetup
{
	public override string InstrumentationMeterName => SimulateWaitTimesMetrics.MeterName;

	public override void RegisterDependencies(IServiceCollection services, IConfigurationManager configurationManager)
	{
		services.AddSingleton<ISimulatorService, SimulateWaitTimesWorker>();
		services.AddHostedService(sp => sp.GetRequiredService<ISimulatorService>());
		services.AddSingleton<SimulateWaitTimesMetrics>();
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
