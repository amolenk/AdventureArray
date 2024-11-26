using AdventureArray.Infrastructure.AppHost.Extensions.AzureCosmosDBPostgres;
using AdventureArray.Infrastructure.AppHost.Extensions.Kafka;
using Aspire.Hosting.Lifecycle;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
	.WithImage("jwhiteatdocker/citus", "12.0.0-pg14")
	.WithDataVolume("postgres-data")
	.WithPgAdmin()
	.WithLifetime(ContainerLifetime.Persistent);

var kafka = builder.AddKafka("kafka")
	.WithLifetime(ContainerLifetime.Persistent) // https://github.com/dotnet/aspire/issues/6651
	.WithDataVolume("kafka-data");
	// .AddTopic("wait_times", 2);

var migrations = builder.AddProject<Projects.Application_MigrationService>("migrations")
	.WithReference(postgres)
	.WaitFor(postgres);

var rideService = builder.AddProject<Projects.Application_RideService>("ride-service")
	.WithReference(postgres)
	.WithReference(kafka)
	.WaitFor(kafka)
	.WaitForCompletion(migrations)
	.WithHttpHealthCheck("/health");

var simulator = builder.AddProject<Projects.Application_Simulator>("simulator")
	.WithReference(postgres)
	.WithReference(kafka)
	.WaitFor(kafka)
	.WaitForCompletion(migrations)
	.WithHttpHealthCheck("/health");

builder.AddProject<Projects.Application_UI>("ui")
	.WithReference(postgres)
	.WithReference(simulator)
	.WaitForCompletion(migrations)
	.WithHttpHealthCheck("/health")
	.WithExternalHttpEndpoints();

builder.Build().Run();
