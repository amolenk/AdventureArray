using System.Reflection;
using AdventureArray.Infrastructure.Features;
using AdventureArray.Infrastructure.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace AdventureArray.Infrastructure.ServiceDefaults;

public static partial class Extensions
{
	public static void AddDefaultFeatures(this IHostApplicationBuilder builder, Assembly scanAssembly)
	{
		var featureManager = CreateFeatureManager(builder.Configuration);

		var featureRegistry = FeatureRegistry.CreateAsync(scanAssembly, featureManager, builder.Configuration)
			.GetAwaiter().GetResult();

		builder.Services.AddSingleton(featureRegistry);
		builder.Properties.Add(nameof(FeatureRegistry), featureRegistry);

		featureRegistry.RegisterDependencies(builder.Services, builder.Configuration);

		ConfigureInstrumentation(builder, featureRegistry);
	}

	public static void MapFeatureEndpoints(this WebApplication app, bool authorize = true)
	{
		var featureRegistry = app.Services.GetService<FeatureRegistry>();
		if (featureRegistry == null)
		{
			throw new InvalidOperationException(
				$"FeatureRegistry is not registered. Make sure {nameof(AddDefaultFeatures)} is called before {nameof(MapFeatureEndpoints)}.");
		}

		foreach (var featureName in featureRegistry.DisabledFeatures)
		{
			app.Logger.LogWarning("Feature {FeatureName} is disabled", featureName);
		}

		// Require authorization for all API endpoints.
		var apiGroup = app.MapGroup("api");
		if (authorize) apiGroup.RequireAuthorization();

		// Add the routes for all registered endpoints.
		foreach (var endpoint in app.Services.GetServices<IApiEndpoint>())
		{
			endpoint.AddRoutes(apiGroup);
		}
	}

	private static void ConfigureInstrumentation(IHostApplicationBuilder builder, FeatureRegistry featureRegistry)
	{
		builder.Services.AddOpenTelemetry()
			.WithMetrics(meterProviderBuilder =>
			{
				foreach (var meterName in featureRegistry.GetInstrumentationMeterNames())
				{
					meterProviderBuilder.AddMeter(meterName);
				}
			});
	}

	private static IFeatureManager CreateFeatureManager(IConfiguration configuration)
	{
		var services = new ServiceCollection();
		services.AddFeatureManagement(configuration.GetSection("Features:Flags"));

		using var serviceProvider = services.BuildServiceProvider();
		return serviceProvider.GetRequiredService<IFeatureManager>();
	}
}
