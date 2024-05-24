using AdventureArray.Infrastructure.Features;

namespace AdventureArray.Application.UI.Features.Simulation.StopSimulator;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class StopSimulatorFeatureSetup : FeatureSetup
{
	public override void RegisterDependencies(IServiceCollection services, IConfigurationManager configurationManager)
	{
		services.AddTransient<IApiEndpoint, StopSimulatorApiEndpoint>();
	}
}
