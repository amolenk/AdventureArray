using System.Diagnostics.CodeAnalysis;
using AdventureArray.Infrastructure.Features;
using AdventureArray.Infrastructure.Routing;
using MassTransit;

namespace AdventureArray.Application.Simulator.Features.Simulation.StopSimulator;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class StopSimulatorFeatureSetup : FeatureSetup
{
	public override void RegisterDependencies(IServiceCollection services, IConfigurationManager configurationManager)
	{
		services.AddTransient<IApiEndpoint, StopSimulatorApiEndpoint>();
	}

	public override void ConfigureMediator(IMediatorRegistrationConfigurator configurator)
	{
		configurator.AddConsumer<StopSimulatorCommandHandler>();
	}
}
