namespace AdventureArray.Application.UI.Client.Features.Simulation.Services;

public interface ISimulationApiService : IApiService
{
	Task<bool> GetSimulatorStatusAsync();

	Task StartSimulatorAsync();

	Task StopSimulatorAsync();
}

public class SimulatorApiService : ISimulationApiService
{
	private readonly HttpClient _httpClient;

	public SimulatorApiService(HttpClient httpClient)
	{
		ArgumentNullException.ThrowIfNull(httpClient);

		_httpClient = httpClient;
	}

	public Task<bool> GetSimulatorStatusAsync()
	{
		return _httpClient.GetFromJsonAsync<bool>("simulator/status");
	}

	public async Task StartSimulatorAsync()
	{
		await _httpClient.PostAsync("simulator/start", null);
	}

	public async Task StopSimulatorAsync()
	{
		await _httpClient.PostAsync("simulator/stop", null);
	}
}
