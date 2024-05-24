using AdventureArray.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AdventureArray.Infrastructure.ServiceDefaults;

public static partial class Extensions
{
	private static void AddDefaultNpgsql(this IHostApplicationBuilder builder)
	{
		ArgumentNullException.ThrowIfNull(builder);

		// // Automatic registration of a pooled DbContext as a scoped service (opinionated defaults).
		// builder.AddNpgsqlDbContext<ApplicationDbContext>("postgres");

		var connectionString = builder.Configuration.GetConnectionString("postgres")
		                       ?? throw new InvalidOperationException("Connection string is not configured.");

		// Register the DbContextFactory as a singleton service.
		builder.Services.AddDbContextFactory<ApplicationDbContext>((_, options) => options
			.UseNpgsql(connectionString));

		// Instrumentation using Aspire settings is added to the DbContext, including retries and timeouts.
		builder.EnrichNpgsqlDbContext<ApplicationDbContext>();
	}
}
