using AdventureArray.Domain.Rides;
using AdventureArray.Infrastructure.Persistence;
using AdventureArray.Infrastructure.Persistence.Customization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
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
		builder.AddNpgsqlDbContext<ApplicationDbContext>("postgres", configureDbContextOptions: options => options
			// Enables migration of partitioned tables
			.ReplaceService<IMigrationsSqlGenerator, CustomMigrationsSqlGenerator>()
			// Add seed data
			.UseSeeding((context, _) =>
			{
				var rides = context.Set<Ride>();

				var testRide = rides.FirstOrDefault(r => r.Id == 1);
				if (testRide != null) return;

				AddRideData(rides);
				context.SaveChanges();
			})
			.UseAsyncSeeding(async (context, _, cancellationToken) =>
			{
				var rides = context.Set<Ride>();

				var testRide = await rides.FirstOrDefaultAsync(r => r.Id == 1, cancellationToken);
				if (testRide != null) return;

				AddRideData(rides);
				await context.SaveChangesAsync(cancellationToken);
			}));
	}

	private static void AddRideData(DbSet<Ride> rides)
	{
		rides.Add(new Ride(1, "Big Whoop Pirate Adventure", RideType.DarkRide, 24, 8, 110, "Binary Bay"));
		rides.Add(new Ride(2, "Data Stream Rapids", RideType.WaterRide, 12, 10, 120, "Cache Cove"));
		rides.Add(new Ride(3, "Quantum Leap Simulator", RideType.SimulatedRide, 30, 4, 140, "Quantum Quadrant"));
		rides.Add(new Ride(4, "Debug Maze", RideType.KiddieRide, 100, 7, 0, "Hello World"));
	}
}
