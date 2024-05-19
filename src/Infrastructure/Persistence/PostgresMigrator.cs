namespace AdventureArray.Infrastructure.Persistence;

public static class PostgresMigrator
{
	public static void MigrateDatabase(ApplicationDbContext context, ILogger logger, DateOnly? startdatumTijdspartities = null)
	{
		startdatumTijdspartities ??= new DateOnly(DateTime.UtcNow.Year, 1, 1);

		// Only update the database if there are pending migrations.
		var pendingMigrations = context.Database.GetPendingMigrations().Any();
		if (pendingMigrations)
		{
			logger.LogInformation("Migrating database...");

			context.Database.Migrate();

			foreach (var entityType in context.Model.GetEntityTypes())
			{
				var tableName = entityType.GetTableName() ?? entityType.Name;

				var isPartitionedTable = entityType.FindAnnotation(CustomDataAnnotations.PartitionByRange) is not null;
				if (!isPartitionedTable) continue;

				var hasPartitions = context.Database.ExecuteSqlRaw(@"
                    SELECT COUNT(*)
                    FROM pg_class
                    WHERE relname = {0} AND relkind = 'p'
                ", tableName) > 0;

				if (!hasPartitions)
				{
					logger.LogInformation("Creating partitions for table {TableName}...", tableName);

					context.Database.SetCommandTimeout(300);
					context.Database.ExecuteSqlRaw(@"
                        SELECT create_time_partitions(
                            table_name => {0},
                            partition_interval => '1 month',
                            end_at => now() + '24 months',
                            start_from => {1});
                        ",
						tableName,
						startdatumTijdspartities);
				}
			}
		}
		else
		{
			logger.LogInformation("Database is up to date, no migration necessary.");
		}
	}
}
