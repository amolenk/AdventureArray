using AdventureArray.Infrastructure.Features;

namespace AdventureArray.Application.UI.Features.Gebruiker.USR001HaalVoorkeurenOp;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class HaalVoorkeurenOpFeatureSetup : FeatureSetup
{
	public override void RegisterDependencies(IServiceCollection services, IConfigurationManager configurationManager)
	{
		services.AddTransient<IApiEndpoint, ApiEndpoint>();
	}

	public override void ConfigureMediator(IMediatorRegistrationConfigurator configurator)
	{
		configurator.AddConsumer<GetVoorkeurenQueryHandler>();
	}
}
