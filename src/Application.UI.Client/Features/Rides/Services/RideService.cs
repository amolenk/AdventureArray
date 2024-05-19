using AdventureArray.Application.UI.Model.Rides;

namespace AdventureArray.Application.UI.Client.Features.Rides.Services;

public interface IRideApiService : IApiService
{
	Task<GetRidesResponse?> GetRidesAsync();
}

public class RideApiService : IRideApiService
{
	private readonly HttpClient _httpClient;

	public RideApiService(HttpClient httpClient)
	{
		ArgumentNullException.ThrowIfNull(httpClient);

		_httpClient = httpClient;
	}

	public Task<GetRidesResponse?> GetRidesAsync()
	{
		return _httpClient.GetFromJsonAsync<GetRidesResponse>("rides");
	}
}
