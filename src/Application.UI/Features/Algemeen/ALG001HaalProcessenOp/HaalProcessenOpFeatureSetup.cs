using AdventureArray.Infrastructure.Features;

namespace AdventureArray.Application.UI.Features.Algemeen.ALG001HaalProcessenOp;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class HaalProcessenOpEanFeatureSetup : FeatureSetup
{
	public override void RegisterDependencies(IServiceCollection services, IConfigurationManager configurationManager)
	{
		services.AddTransient<IApiEndpoint, HaalProcessenOpApiEndpoint>();
	}

	public override void ConfigureMediator(IMediatorRegistrationConfigurator configurator)
	{
		configurator.AddConsumer<HaalProcessenOpQueryHandler>();
	}
}
