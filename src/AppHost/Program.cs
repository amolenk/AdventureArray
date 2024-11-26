using AdventureArray.Infrastructure.AppHost.Extensions.AzureCosmosDBPostgres;
using AdventureArray.Infrastructure.AppHost.Extensions.Kafka;
using Aspire.Hosting.Lifecycle;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
	.WithImage("jwhiteatdocker/citus", "12.0.0-pg14")
	.WithDataVolume("postgres-data")
	.WithPgAdmin();
	// .WithLifetime(ContainerLifetime.Persistent);
//
// var kafka = builder.AddKafka("kafka")
// 	.WithLifetime(ContainerLifetime.Persistent)
// 	.AddTopic("wait_times", 2);
//
// var rideService = builder.AddProject<Projects.Application_RideService>("ride-service")
// 	.WithReference(postgres)
// 	.WithReference(kafka);
//
// var simulator = builder.AddProject<Projects.Application_Simulator>("simulator")
// 	.WithReference(postgres)
// 	.WithReference(kafka);
//
// builder.AddProject<Projects.Application_UI>("ui")
// 	.WithReference(postgres)
// 	.WithReference(simulator)
// 	.WithExternalHttpEndpoints();
//
builder.AddProject<Projects.Application_MigrationService>("migrations")
	.WithReference(postgres)
	.WaitFor(postgres);

builder.Build().Run();
