namespace AdventureArray.Application.Simulator.Features.Simulation;

public interface ISimulatorService : IHostedService
{
	public bool IsRunning { get; }
}
