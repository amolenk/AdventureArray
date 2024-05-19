using AdventureArray.Infrastructure.Features;

namespace AdventureArray.Application.UI.Features.Rides.RDE001GetRides;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class GetRidesFeatureSetup : FeatureSetup
{
	public override void RegisterDependencies(IServiceCollection services, IConfigurationManager configurationManager)
	{
		services.AddTransient<IApiEndpoint, GetRidesApi>();
	}

	public override void ConfigureMediator(IMediatorRegistrationConfigurator configurator)
	{
		configurator.AddConsumer<GetRidesQueryHandler>();
	}
}
