using AdventureArray.Infrastructure.Features;

namespace AdventureArray.Application.UI.Features.Simulation.StartSimulator;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class StartSimulatorFeatureSetup : FeatureSetup
{
	public override void RegisterDependencies(IServiceCollection services, IConfigurationManager configurationManager)
	{
		services.AddTransient<IApiEndpoint, StartSimulatorApiEndpoint>();
	}
}
