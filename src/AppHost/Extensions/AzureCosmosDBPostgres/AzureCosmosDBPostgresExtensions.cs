namespace AdventureArray.Infrastructure.AppHost.Extensions.AzureCosmosDBPostgres;

/// <summary>
/// Provides extension methods for adding the Azure Event Hubs resources to the application model.
/// </summary>
public static class AzureCosmosDBPostgresExtensions
{
	public static IResourceBuilder<AzureCosmosDBPostgresServerResource> AddAzureCosmosDBPostgres(
		this IDistributedApplicationBuilder builder,
		string name,
		IResourceBuilder<ParameterResource>? password = null)
	{
		var passwordParameter = password?.Resource ??
		                        ParameterResourceBuilderExtensions.CreateDefaultPasswordParameter(
			                        builder, $"{name}-password");

		var resource = new AzureCosmosDBPostgresServerResource(name);

		return builder.AddResource(resource)
			.WithManifestPublishingCallback(resource.WriteToManifest)
			.WithParameter("cosmosDbPassword", passwordParameter);
	}
}
