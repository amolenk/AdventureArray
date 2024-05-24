namespace AdventureArray.Application.UI.Features.Simulation.GetSimulatorStatus;

public class GetSimulatorStatusApiEndpoint : IApiEndpoint
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapGet("simulator/status", async (SimulatorServiceClient client) =>
			{
				var isRunning = await client.IsSimulatorRunningAsync();

				return Results.Ok(isRunning);
			})
			.WithName("GetSimulatorStatus")
			.Produces<bool>();
	}
}
