using Aspire.Hosting.Azure;

namespace AdventureArray.Infrastructure.AppHost.Extensions.AzureCosmosDBPostgres;

public class AzureCosmosDBPostgresServerResource(string name)
	: AzureBicepResource(name, templateFile: BicepTemplateFile), IResourceWithConnectionString
{
	private const string BicepTemplateFile = "Extensions/AzureCosmosDBPostgres/template.bicep";

	/// <summary>
	/// Gets the connection string template for the manifest for the Azure Event Hubs endpoint.
	/// </summary>
	public ReferenceExpression ConnectionStringExpression => ReferenceExpression.Create($"{ConnectionString}");

	/// <summary>
	/// Gets the "connectionString" output reference from the bicep template.
	/// </summary>
	private BicepSecretOutputReference ConnectionString => new("connectionString", this);
}
