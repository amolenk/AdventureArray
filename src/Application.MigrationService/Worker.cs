using System.Diagnostics;
using AdventureArray.Domain.Rides;
using AdventureArray.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using OpenTelemetry.Trace;

namespace AdventureArray.Application.MigrationService;

public class Worker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = s_activitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await EnsureDatabaseAsync(dbContext, stoppingToken);
            await RunMigrationAsync(dbContext, stoppingToken);
            await SeedDataAsync(dbContext, stoppingToken);
        }
        catch (Exception ex)
        {
            activity?.RecordException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task EnsureDatabaseAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Create the database if it does not exist.
            // Do this first so there is then a database to start a transaction against.
            if (!await dbCreator.ExistsAsync(cancellationToken))
            {
                await dbCreator.CreateAsync(cancellationToken);
            }
        });
    }

    private static async Task RunMigrationAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Run migration in a transaction to avoid partial migration if it fails.
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            await dbContext.Database.MigrateAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });
    }

    private static async Task SeedDataAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
	    Ride[] rides =
	    [
		    new Ride(1, "Big Whoop Pirate Adventure", RideType.DarkRide, 24, 8, 110, "Binary Bay"),
		    new Ride(2, "Data Stream Rapids", RideType.WaterRide, 12, 10, 120, "Cache Cove"),
		    new Ride(3, "Quantum Leap Simulator", RideType.SimulatedRide, 30, 4, 140, "Quantum Quadrant"),
		    new Ride(4, "Debug Maze", RideType.KiddieRide, 100, 7, 0, "Hello World")
	    ];

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Seed the database
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            if (!await dbContext.Rides.AnyAsync(cancellationToken))
            {
	            await dbContext.Rides.AddRangeAsync(rides, cancellationToken);
	            await dbContext.SaveChangesAsync(cancellationToken);
            }
            await transaction.CommitAsync(cancellationToken);
        });
    }
}
