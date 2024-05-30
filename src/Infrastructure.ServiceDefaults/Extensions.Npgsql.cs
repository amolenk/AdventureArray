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

		// Automatic registration of a pooled DbContext as a scoped service (opinionated defaults).
		builder.AddNpgsqlDbContext<ApplicationDbContext>("postgres");
	}
}
