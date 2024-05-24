namespace AdventureArray.Application.UI.Features.Simulation.StopSimulator;

public class StopSimulatorApiEndpoint : IApiEndpoint
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapPost("simulator/stop", async (SimulatorServiceClient client) =>
			{
				await client.StopSimulatorAsync();

				return Results.Ok();
			})
			.WithName("StopSimulator");
	}
}
