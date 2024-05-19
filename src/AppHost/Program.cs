using AdventureArray.Infrastructure.AppHost.Kafka;

var builder = DistributedApplication.CreateBuilder(args);

var kafka = builder.AddKafka("kafka", port: 6000)
	.WithTopic("wait_times", 2).WithTopic("ride_requests", 4);

var citus = builder.AddPostgres("citus")
	.WithImage("jwhiteatdocker/citus", "12.0.0-pg14")
	.WithPgAdmin();

var rideService = builder.AddProject<Projects.Application_RideService>("rideService")
	.WithReference(citus)
	.WithReference(kafka)
	.WithReplicas(2);

var simulator = builder.AddProject<Projects.Application_Simulator>("simulator")
	.WithReference(citus)
	.WithReference(kafka);

builder.AddProject<Projects.Application_UI>("ui")
    .WithExternalHttpEndpoints()
    .WithReference(citus);
    // .WithReference(simulator);

builder.AddProject<Projects.Application_MigrationService>("migrations")
	.WithReference(citus);

builder.Build().Run();
