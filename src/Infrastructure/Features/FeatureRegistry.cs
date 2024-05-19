using AdventureArray.Infrastructure.Messaging.Configuration;

namespace AdventureArray.Infrastructure.Features;

/// <summary>
/// Used to initialize features at application startup.
/// </summary>
public class FeatureRegistry
{
	private readonly List<string> _disabledFeatures;
	private readonly List<IFeatureSetup> _featureSetups;

	private FeatureRegistry(List<IFeatureSetup> featureSetups, List<string> disabledFeatures)
	{
		_featureSetups = featureSetups;
		_disabledFeatures = disabledFeatures;
	}

	public IEnumerable<string> DisabledFeatures => _disabledFeatures;

	public static async Task<FeatureRegistry> CreateAsync(Assembly assembly, IFeatureManager featureManager,
		IConfigurationManager configuration)
	{
		List<IFeatureSetup> featureSetups = [];
		List<string> disabledFeatures = [];

		foreach (var featureSetup in CreateFeatureSetups(assembly))
			if (featureSetup.FeatureFlagName is not null
				&& !await featureManager.IsEnabledAsync(featureSetup.FeatureFlagName))
				disabledFeatures.Add(featureSetup.FeatureFlagName);
			else
				featureSetups.Add(featureSetup);

		return new FeatureRegistry(featureSetups, disabledFeatures);
	}

	public IEnumerable<string> GetInstrumentationMeterNames()
	{
		return _featureSetups
			.Where(setup => setup.InstrumentationMeterName != null)
			.Select(setup => setup.InstrumentationMeterName!);
	}

	public void RegisterDependencies(IServiceCollection services, IConfigurationManager configurationManager)
	{
		ArgumentNullException.ThrowIfNull(services);
		ArgumentNullException.ThrowIfNull(configurationManager);

		foreach (var featureSetup in _featureSetups) featureSetup.RegisterDependencies(services, configurationManager);
	}

	public void ConfigureMediator(IMediatorRegistrationConfigurator configurator)
	{
		ArgumentNullException.ThrowIfNull(configurator);

		foreach (var featureSetup in _featureSetups) featureSetup.ConfigureMediator(configurator);
	}

	public void ConfigureBus(IBusRegistrationConfigurator configurator)
	{
		ArgumentNullException.ThrowIfNull(configurator);

		foreach (var featureSetup in _featureSetups) featureSetup.ConfigureBus(configurator);
	}

	public void ConfigureRider(IRiderRegistrationConfigurator configurator, IConfiguration configuration)
	{
		ArgumentNullException.ThrowIfNull(configurator);

		foreach (var featureSetup in _featureSetups) featureSetup.ConfigureRider(configurator, configuration);
	}

	public void ConfigureRiderEndpoints(IRiderRegistrationContext context, IKafkaFactoryConfigurator configurator)
	{
		ArgumentNullException.ThrowIfNull(configurator);

		foreach (var featureSetup in _featureSetups)
			featureSetup.ConfigureRiderEndpoints(context, configurator);
	}

	private static IEnumerable<IFeatureSetup> CreateFeatureSetups(Assembly assembly)
	{
		var typesFromCallingAssembly = assembly.GetTypes();
		var typesImplementingIFeatureSetup = typesFromCallingAssembly
			.Where(type => typeof(IFeatureSetup).IsAssignableFrom(type));

		foreach (var type in typesImplementingIFeatureSetup)
			if (Activator.CreateInstance(type) is IFeatureSetup instance)
				yield return instance;
	}
}
