using System.Diagnostics.CodeAnalysis;
using AdventureArray.Infrastructure.Features;
using AdventureArray.Infrastructure.Routing;

namespace AdventureArray.Application.RideService.Features.Diagnostics.Ping;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class PingFeatureSetup : FeatureSetup
{
	public override void RegisterDependencies(IServiceCollection services, IConfigurationManager configurationManager)
	{
		services.AddTransient<IApiEndpoint, PingApiEndpoint>();
	}
}
