using System.Diagnostics.CodeAnalysis;
using AdventureArray.Infrastructure.Routing;
using MassTransit;
using MassTransit.Mediator;

namespace AdventureArray.Application.Simulator.Features.Simulation.StopSimulator;

public class StopSimulatorApiEndpoint : IApiEndpoint
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapPost("simulator/stop", async (IScopedMediator mediator) =>
			{
				await mediator.Send(new StopSimulatorCommand());
				return Results.Ok();
			})
			.WithName("StopSimulator");
	}
}

public record StopSimulatorCommand;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class StopSimulatorCommandHandler : IConsumer<StopSimulatorCommand>
{
	private readonly ISimulatorService? _simulatorService;

	public StopSimulatorCommandHandler(ISimulatorService? simulatorService)
	{
		_simulatorService = simulatorService;
	}

	public async Task Consume(ConsumeContext<StopSimulatorCommand> context)
	{
		if (_simulatorService is { IsRunning: true })
		{
			await _simulatorService.StopAsync(context.CancellationToken);
		}
	}
}
