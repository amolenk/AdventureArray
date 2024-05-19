namespace AdventureArray.Infrastructure.Persistence.Customization;

/// <summary>
/// This class is responsible for generating SQL commands for migrations in a Citus database.
/// It extends the base MigrationsSqlGenerator class provided by Entity Framework Core.
/// </summary>
/// <remarks>
/// The class overrides the Generate method for CreateTableOperation to handle partitioning by range in Citus.
/// </remarks>
public class CustomMigrationsSqlGenerator(
	MigrationsSqlGeneratorDependencies dependencies,
	[SuppressMessage("Usage", "EF1001:Internal EF Core API usage.")] INpgsqlSingletonOptions npgsqlSingletonOptions)
	: NpgsqlMigrationsSqlGenerator(dependencies, npgsqlSingletonOptions)
{
	private bool _citusExtensionEmitted;

	protected override void Generate(MigrationOperation operation, IModel? model, MigrationCommandListBuilder builder)
	{
		var citusAnnotation = model?.FindAnnotation(CustomDataAnnotations.EnsureCitusExtension);
		if (citusAnnotation?.Value is true && !_citusExtensionEmitted)
		{
			builder.AppendLine("CREATE EXTENSION IF NOT EXISTS citus;");
			builder.AppendLine();

			_citusExtensionEmitted = true;
		}

		base.Generate(operation, model, builder);
	}

	protected override void Generate(CreateTableOperation operation, IModel? model, MigrationCommandListBuilder builder,
		bool terminate = true)
	{
		base.Generate(operation, model, builder, false);

		// Find the entity for the current table.
		var entity = model?.GetEntityTypes().FirstOrDefault(e => e.GetTableName() == operation.Name);

		if (entity == null) return;

		// If the PartitionByRange annotation is set, add the partitioning clause to the table definition.
		var partitionByRangeAnnotation = entity.FindAnnotation(CustomDataAnnotations.PartitionByRange);
		if (partitionByRangeAnnotation?.Value is not null)
		{
			var partitionKey = NpgsqlSnakeCaseNameTranslator.ConvertToSnakeCase(
				partitionByRangeAnnotation.Value.ToString()!,
				CultureInfo.InvariantCulture);

			builder.Append($" PARTITION BY RANGE ({partitionKey})");
		}

		// End the table definition.
		if (terminate)
		{
			builder.AppendLine(";");
			EndStatement(builder);
		}

		var distributeAnnotation = entity.FindAnnotation(CustomDataAnnotations.Distribute);
		if (distributeAnnotation?.Value is not null)
		{
			var distributeColumn = NpgsqlSnakeCaseNameTranslator.ConvertToSnakeCase(
				distributeAnnotation.Value.ToString()!,
				CultureInfo.InvariantCulture);

			var colocatedAnnotation = entity.FindAnnotation(CustomDataAnnotations.Colocated);
			if (colocatedAnnotation?.Value is not null)
			{
				var colocatedTableName = colocatedAnnotation.Value.ToString()!.ToLowerInvariant();

				builder.AppendLine($"SELECT create_distributed_table('{operation.Name}', '{distributeColumn}', colocate_with => '{colocatedTableName}');");
				EndStatement(builder);
			}
			else
			{
				builder.AppendLine($"SELECT create_distributed_table('{operation.Name}', '{distributeColumn}');");
				EndStatement(builder);
			}
		}

		var referenceAnnotation = entity.FindAnnotation(CustomDataAnnotations.Reference);
		if (referenceAnnotation is not null)
		{
			builder.AppendLine($"SELECT create_reference_table('{operation.Name}');");
			EndStatement(builder);
		}
	}
}
