using System.Diagnostics.CodeAnalysis;
using AdventureArray.Infrastructure.Features;
using AdventureArray.Infrastructure.Routing;
using MassTransit;

namespace AdventureArray.Application.Simulator.Features.Simulation.GetSimulatorStatus;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class GetSimulatorStatusFeatureSetup : FeatureSetup
{
	public override void RegisterDependencies(IServiceCollection services, IConfigurationManager configurationManager)
	{
		services.AddTransient<IApiEndpoint, GetSimulatorStatusApiEndpoint>();
	}

	public override void ConfigureMediator(IMediatorRegistrationConfigurator configurator)
	{
		configurator.AddConsumer<GetSimulatorStatusQueryHandler>();
	}
}
