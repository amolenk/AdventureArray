using AdventureArray.Infrastructure.Messaging.Configuration;

namespace AdventureArray.Infrastructure.Features;

public interface IFeatureSetup
{
	/// <summary>
	/// De naam van de feature flag.
	/// </summary>
	string? FeatureFlagName { get; }

	/// <summary>
	/// De naam van de meter voor het bijhouden van metrieken (bv. adventure-array.xyz001
	/// </summary>
	string? InstrumentationMeterName { get; }

	/// <summary>
	/// Registreert alle afhankelijkheden van de feature in de IoC container.
	/// </summary>
	void RegisterDependencies(IServiceCollection services, IConfigurationManager configurationManager);

	/// <summary>
	/// Voegt configuratie toe aan de MassTransit mediator.
	/// </summary>
	void ConfigureMediator(IMediatorRegistrationConfigurator configurator);

	/// <summary>
	/// Voegt configuratie toe aan de MassTransit bus.
	/// </summary>
	void ConfigureBus(IBusRegistrationConfigurator configurator);

	/// <summary>
	/// Voegt configuratie toe aan de MassTransit rider.
	/// </summary>
	void ConfigureRider(IRiderRegistrationConfigurator configurator, IConfiguration configuration);

	/// <summary>
	/// Voegt Kafka-specifieke configuratie toe aan de MassTransit rider.
	/// </summary>
	void ConfigureRiderEndpoints(IRiderRegistrationContext context, IKafkaFactoryConfigurator configurator);
}

public class FeatureSetup : IFeatureSetup
{
	public virtual string? FeatureFlagName => null;
	public virtual string? InstrumentationMeterName => null;

	/// <inheritdoc />
	public virtual void RegisterDependencies(IServiceCollection services, IConfigurationManager configurationManager)
	{
	}

	/// <inheritdoc />
	public virtual void ConfigureMediator(IMediatorRegistrationConfigurator configurator)
	{
	}

	/// <inheritdoc />
	public virtual void ConfigureBus(IBusRegistrationConfigurator configurator)
	{
	}

	/// <inheritdoc />
	public virtual void ConfigureRider(IRiderRegistrationConfigurator configurator, IConfiguration configuration)
	{
	}

	/// <inheritdoc />
	public virtual void ConfigureRiderEndpoints(IRiderRegistrationContext context, IKafkaFactoryConfigurator configurator)
	{
	}
}
