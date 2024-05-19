using AdventureArray.Infrastructure.Routing;

namespace AdventureArray.Infrastructure.Features.Extensions;

public static class WebApplicationExtensions
{
	public static void UseCustomFeatures(this WebApplication webApplication, bool disableApiAuthorization = false)
	{
		var featureRegistry = webApplication.Services.GetService<FeatureRegistry>();
		if (featureRegistry == null)
		{
			throw new InvalidOperationException(
				"FeatureRegistry is not registered. Make sure AddCustomFeatures is called before UseCustomFeatures.");
		}

		foreach (var featureName in featureRegistry.DisabledFeatures)
		{
			webApplication.Logger.LogWarning("Feature {FeatureName} is disabled", featureName);
		}

		MapCustomApiEndpoints(webApplication, disableApiAuthorization);
	}

	private static void MapCustomApiEndpoints(WebApplication app, bool disableAuthorization)
	{
		// Require authorization for all API endpoints.
		var apiGroup = app.MapGroup("api");

		// if (disableAuthorization)
		// {
		// 	app.Logger.LogWarning("Autorisatie op API endpoints is uitgeschakeld.");
		// }
		// else
		// {
		// 	apiGroup.RequireAuthorization();
		// }

		// Add the routes for all registered endpoints.
		foreach (var endpoint in app.Services.GetServices<IApiEndpoint>())
		{
			endpoint.AddRoutes(apiGroup);
		}
	}
}
