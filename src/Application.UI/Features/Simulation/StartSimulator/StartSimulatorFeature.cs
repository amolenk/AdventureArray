namespace AdventureArray.Application.UI.Features.Simulation.StartSimulator;

public class StartSimulatorApiEndpoint : IApiEndpoint
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapPost("simulator/start", async (SimulatorServiceClient client) =>
			{
				await client.StartSimulatorAsync();

				return Results.Ok();
			})
			.WithName("StartSimulator");
	}
}
