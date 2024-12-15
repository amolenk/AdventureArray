using AdventureArray.Infrastructure.AppHost.Extensions.AzureCosmosDBPostgres;
using AdventureArray.Infrastructure.AppHost.Extensions.Kafka;
using Aspire.Hosting.Lifecycle;
using Microsoft.AspNetCore.Builder;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
	.WithImage("jwhiteatdocker/citus", "12.0.0-pg14")
	.WithDataVolume("postgres-data")
	.WithPgAdmin()
	.WithLifetime(ContainerLifetime.Persistent);

var kafka = builder.AddKafka("kafka")
	.WithDataVolume("kafka-data")
	.WithKafkaUI()
	.WithLifetime(ContainerLifetime.Persistent) // https://github.com/dotnet/aspire/issues/6651
	.AddTopic("wait_times", 2);

var migrations = builder.AddProject<Application_MigrationService>("migrations")
	.WithReference(postgres)
	.WaitFor(postgres);

var simulator = builder.AddProject<Application_Simulator>("simulator")
	.WithReference(postgres)
	.WithReference(kafka)
	.WaitFor(kafka)
	.WaitForCompletion(migrations)
	.WithHttpHealthCheck("/health");

var rideService = builder.AddProject<Application_RideService>("ride-service")
	.WithReference(postgres)
	.WithReference(kafka)
	.WaitFor(kafka)
	.WaitForCompletion(migrations)
	.WithHttpHealthCheck("/health");

builder.AddProject<Application_UI>("ui")
	.WithReference(postgres)
	.WithReference(simulator)
	.WaitForCompletion(migrations)
	.WithHttpHealthCheck("/health")
	.WithExternalHttpEndpoints()
	.PublishAsAzureContainerApp((module, containerApp) =>
	{
		containerApp.Template.Scale.MinReplicas = 0;
	});

builder.Build().Run();
