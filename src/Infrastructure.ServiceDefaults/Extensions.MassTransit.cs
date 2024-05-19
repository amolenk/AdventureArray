using AdventureArray.Infrastructure.Features;
using AdventureArray.Infrastructure.Messaging.Configuration;
using AdventureArray.Infrastructure.Messaging.RetryPolicies;
using AdventureArray.Infrastructure.Messaging.Topology;
using Confluent.Kafka;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AdventureArray.Infrastructure.ServiceDefaults;

public static partial class Extensions
{
	private static void AddDefaultMassTransit(this IHostApplicationBuilder builder,
		Action<IBusRegistrationConfigurator>? configure = null)
	{
		ArgumentNullException.ThrowIfNull(builder);

		if (!builder.Properties.TryGetValue(nameof(FeatureRegistry), out var property))
		{
			throw new InvalidOperationException(
				"FeatureRegistry is not registered. Make sure AddDefaultFeatures is called before AddDefaultMassTransit.");
		}
		var featureRegistry = (FeatureRegistry)property;

		builder.Services.AddMassTransit(bus =>
		{
			configure?.Invoke(bus);

			AddDefaultMassTransitBus(builder, bus, featureRegistry);
			AddDefaultMassTransitRider(builder, bus, featureRegistry);
		});

		builder.Services.AddMediator(cfg =>
		{
			featureRegistry.ConfigureMediator(cfg);
		});

		// builder.Services.AddOptions<MassTransitHostOptions>()
		// 	.Configure(options =>
		// 	{
		// 		// if specified, waits until the bus is started before
		// 		// returning from IHostedService.StartAsync
		// 		// default is false
		// 		options.WaitUntilStarted = true;
		// 	});
	}

	private static void AddDefaultMassTransitBus(IHostApplicationBuilder builder, IBusRegistrationConfigurator bus,
		FeatureRegistry featureRegistry)
	{
		featureRegistry.ConfigureBus(bus);

		var serviceBusConnectionString = builder.Configuration.GetConnectionString("AzureServiceBus");
		if (serviceBusConnectionString is not null)
		{
			// Use the built-in dead-letter queue instead of moving messages to the _skipped or _error queues.
			bus.AddConfigureEndpointsCallback((_, cfg) =>
			{
				if (cfg is not IServiceBusReceiveEndpointConfigurator sb) return;
				sb.ConfigureDeadLetterQueueDeadLetterTransport();
				sb.ConfigureDeadLetterQueueErrorTransport();
			});

			bus.UsingAzureServiceBus((context, cfg) =>
			{
				cfg.Host(serviceBusConnectionString);
				cfg.UseMessageRetry(retry => retry.ConfigureInfiniteRetryPolicy());
				cfg.MessageTopology.SetEntityNameFormatter(new CustomEntityNameFormatter());
				cfg.ConfigureEndpoints(context);
			});

			return;
		}

		var rabbitMqConnectionString = builder.Configuration.GetConnectionString("RabbitMQ");
		if (rabbitMqConnectionString is not null)
		{
			bus.UsingRabbitMq((context, cfg) =>
			{
				cfg.Host(rabbitMqConnectionString);
				cfg.UseMessageRetry(retry => retry.ConfigureInfiniteRetryPolicy());
				cfg.MessageTopology.SetEntityNameFormatter(new CustomEntityNameFormatter());
				cfg.ConfigureEndpoints(context);
			});

			return;
		}

		bus.UsingInMemory();
	}

	private static void AddDefaultMassTransitRider(IHostApplicationBuilder builder, IBusRegistrationConfigurator bus,
		FeatureRegistry featureRegistry)
	{
		var kafkaConnectionString = builder.Configuration.GetConnectionString("kafka");
		if (kafkaConnectionString is null) return;

		// var kafkaSection = builder.Configuration.GetSection("Kafka");
		// if (!kafkaSection.Exists()) return;
		//
		// KafkaSettings kafkaSettings = new();
		// kafkaSection.Bind(kafkaSettings);

		bus.AddRider(rider =>
		{
			featureRegistry.ConfigureRider(rider, builder.Configuration);

			rider.UsingKafka((context, k) =>
			{
				k.Host([kafkaConnectionString], configureKafkaHost =>
				{
					// // var saslUsername = kafkaSettings.SaslUsername;
					// // if (saslUsername is null) return;
					// //
					// configureKafkaHost.UseSasl(sasl =>
					// {
					// 	sasl.Mechanism = SaslMechanism.Plain;
					// 	sasl.SecurityProtocol = SecurityProtocol.SaslSsl;
					// 	// sasl.Username = saslUsername;
					// 	// sasl.Password = kafkaSettings.SaslPassword;
					// });
				});

				featureRegistry.ConfigureRiderEndpoints(context, k);
			});
		});
	}
}
