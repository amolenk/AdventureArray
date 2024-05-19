namespace AdventureArray.Infrastructure.Storage.Extensions;

/// <summary>
/// Extension methods for <see cref="IHostApplicationBuilder"/>.
/// </summary>
public static class HostApplicationBuilderExtensions
{
	/// <summary>
	/// Register services for Azure Storage.
	/// </summary>
	public static void AddCustomAzureStorage(this IHostApplicationBuilder builder)
	{
		ArgumentNullException.ThrowIfNull(builder);

		var connectionString = builder.Configuration.GetConnectionString("BlobStorage");
		if (connectionString is not null)
		{
			builder.Services.Configure<BlobStorageSettings>(
				options => options.ConnectionString = connectionString);
			builder.Services.AddScoped<IBlobService, BlobService>();
		}
	}
}
