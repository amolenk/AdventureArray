using AdventureArray.Infrastructure.Features;

namespace AdventureArray.Application.UI.Features.Gebruiker.USR002WijzigVoorkeuren;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class WijzigVoorkeurenFeatureSetup : FeatureSetup
{
	public override void RegisterDependencies(IServiceCollection services, IConfigurationManager configurationManager)
	{
		services.AddTransient<IApiEndpoint, ApiEndpoint>();
	}

	public override void ConfigureMediator(IMediatorRegistrationConfigurator configurator)
	{
		configurator.AddConsumer<UpdateVoorkeurenHandler>();
	}
}
