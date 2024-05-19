namespace AdventureArray.Infrastructure.Persistence.Extensions;

/// <summary>
/// Extension methods for <see cref="IHostApplicationBuilder"/>.
/// </summary>
public static class HostApplicationBuilderExtensions
{
	/// <summary>
	/// Configures PostgreSQL with Entity Framework Core.
	/// </summary>
	public static void AddCustomPostgreSql(this IHostApplicationBuilder builder)
	{
		ArgumentNullException.ThrowIfNull(builder);

		var connectionString = builder.Configuration.GetConnectionString("Database");
		if (string.IsNullOrWhiteSpace(connectionString))
		{
			throw new InvalidOperationException("Database connection string is not configured.");
		}

		builder.Services.AddDbContextFactory<ApplicationDbContext>((_, options) => options
			.UseNpgsql(connectionString, npgsqlDbContextOptionsBuilder =>
				npgsqlDbContextOptionsBuilder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), default)
			)
			.UseNpgsql(connectionString)
			.UseSnakeCaseNamingConvention()
			// Enables migration of partitioned tables.
			.ReplaceService<IMigrationsSqlGenerator, CustomMigrationsSqlGenerator>());
	}
}
