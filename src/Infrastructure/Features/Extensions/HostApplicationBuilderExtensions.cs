namespace AdventureArray.Infrastructure.Features.Extensions;

public static class HostApplicationBuilderExtensions
{
	public static void AddCustomFeatures(this IHostApplicationBuilder builder, Assembly scanAssembly)
	{
		var featureManager = CreateFeatureManager(builder.Configuration);

		var featureRegistry = FeatureRegistry.CreateAsync(scanAssembly, featureManager, builder.Configuration)
			.GetAwaiter().GetResult();

		builder.Services.AddSingleton(featureRegistry);
		builder.Properties.Add(nameof(FeatureRegistry), featureRegistry);

		featureRegistry.RegisterDependencies(builder.Services, builder.Configuration);
	}

	private static IFeatureManager CreateFeatureManager(IConfiguration configuration)
	{
		var services = new ServiceCollection();
		services.AddFeatureManagement(configuration.GetSection("Features:Flags"));

		using var serviceProvider = services.BuildServiceProvider();
		return serviceProvider.GetRequiredService<IFeatureManager>();
	}
}
