using System.Diagnostics.CodeAnalysis;
using AdventureArray.Application.Simulator.Features.Simulation.StopSimulator;
using AdventureArray.Infrastructure.Features;
using AdventureArray.Infrastructure.Routing;
using MassTransit;

namespace AdventureArray.Application.Simulator.Features.Simulation.StartSimulator;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class StartSimulatorFeatureSetup : FeatureSetup
{
	public override void RegisterDependencies(IServiceCollection services, IConfigurationManager configurationManager)
	{
		services.AddTransient<IApiEndpoint, StartSimulatorApiEndpoint>();
	}

	public override void ConfigureMediator(IMediatorRegistrationConfigurator configurator)
	{
		configurator.AddConsumer<StartSimulatorCommandHandler>();
	}
}
