using AdventureArray.Infrastructure.Persistence;
using AdventureArray.Infrastructure.Persistence.Customization;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Hosting;

namespace AdventureArray.Infrastructure.ServiceDefaults;

public static partial class Extensions
{
	private static void AddDefaultNpgsql(this IHostApplicationBuilder builder)
	{
		ArgumentNullException.ThrowIfNull(builder);

		// Automatic registration of a pooled DbContext as a scoped service (opinionated defaults).
		builder.AddNpgsqlDbContext<ApplicationDbContext>("postgres");//, configureDbContextOptions: options => options
				// .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning))
				// Enables migration of partitioned tables.
				//.ReplaceService<IMigrationsSqlGenerator, CustomMigrationsSqlGenerator>());
	}
}
