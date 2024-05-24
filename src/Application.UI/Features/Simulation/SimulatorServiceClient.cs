namespace AdventureArray.Application.UI.Features.Simulation;

public class SimulatorServiceClient
{
	private readonly HttpClient _httpClient;

	public SimulatorServiceClient(HttpClient httpClient)
	{
		ArgumentNullException.ThrowIfNull(httpClient);

		_httpClient = httpClient;
	}

	public async Task<bool> IsSimulatorRunningAsync()
	{
		var status = await _httpClient.GetFromJsonAsync<SimulatorStatus>("api/simulator/status");
		return (bool)status?.IsRunning;
	}

	public async Task StartSimulatorAsync()
	{
		await _httpClient.PostAsync("api/simulator/start", null);
	}

	public async Task StopSimulatorAsync()
	{
		await _httpClient.PostAsync("api/simulator/stop", null);
	}
}

public record SimulatorStatus(bool IsRunning);
