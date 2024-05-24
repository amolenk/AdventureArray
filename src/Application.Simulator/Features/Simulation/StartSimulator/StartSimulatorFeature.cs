using System.Diagnostics.CodeAnalysis;
using AdventureArray.Infrastructure.Routing;
using MassTransit;
using MassTransit.Mediator;

namespace AdventureArray.Application.Simulator.Features.Simulation.StartSimulator;

public class StartSimulatorApiEndpoint : IApiEndpoint
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapPost("simulator/start", async (IScopedMediator mediator) =>
			{
				await mediator.Send(new StartSimulatorCommand());
				return Results.Ok();
			})
			.WithName("StartSimulator");
	}
}

public record StartSimulatorCommand;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class StartSimulatorCommandHandler : IConsumer<StartSimulatorCommand>
{
	private readonly ISimulatorService? _simulatorService;

	public StartSimulatorCommandHandler(ISimulatorService? simulatorService)
	{
		_simulatorService = simulatorService;
	}

	public async Task Consume(ConsumeContext<StartSimulatorCommand> context)
	{
		if (_simulatorService is { IsRunning: false })
		{
			await _simulatorService.StartAsync(context.CancellationToken);
		}
	}
}
