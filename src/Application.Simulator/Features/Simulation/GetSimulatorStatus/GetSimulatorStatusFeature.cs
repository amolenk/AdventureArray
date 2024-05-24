using System.Diagnostics.CodeAnalysis;
using AdventureArray.Infrastructure.Messaging.Extensions;
using AdventureArray.Infrastructure.Routing;
using MassTransit;
using MassTransit.Mediator;

namespace AdventureArray.Application.Simulator.Features.Simulation.GetSimulatorStatus;

public class GetSimulatorStatusApiEndpoint : IApiEndpoint
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapGet("simulator/status", async (IScopedMediator mediator) =>
			{
				var query = new GetSimulatorStatusQuery();
				var result = await mediator.Send<GetSimulatorStatusQuery, GetSimulatorStatusResponse>(query);
				return Results.Ok(result);
			})
			.WithName("GetSimulatorStatus")
			.Produces<GetSimulatorStatusResponse>();
	}
}

public record GetSimulatorStatusQuery;

public record GetSimulatorStatusResponse(bool isRunning);

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class GetSimulatorStatusQueryHandler : IConsumer<GetSimulatorStatusQuery>
{
	private readonly ISimulatorService? _simulatorService;

	public GetSimulatorStatusQueryHandler(ISimulatorService? simulatorService)
	{
		_simulatorService = simulatorService;
	}

	public async Task Consume(ConsumeContext<GetSimulatorStatusQuery> context)
	{
		var isRunning = _simulatorService?.IsRunning ?? false;

		await context.RespondAsync(new GetSimulatorStatusResponse(isRunning));
	}
}
