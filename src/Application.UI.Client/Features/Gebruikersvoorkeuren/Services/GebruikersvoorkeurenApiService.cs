using AdventureArray.Application.UI.Model.Gebruiker;

namespace AdventureArray.Application.UI.Client.Features.Gebruikersvoorkeuren.Services;

/// <summary>
/// Client for the preferences API.
/// </summary>
public interface IGebruikersvoorkeurenApiService : IApiService
{
	Task<GetGebruikersvoorkeurenResponse?> GetGebruikersvoorkeurenAsync();
	Task<UpdateGebruikersvoorkeurenReponse?> UpdateGebruikersvoorkeurenAsync(UpdateGebruikersvoorkeurenRequest request);
}

public class GebruikersvoorkeurenApiService : IGebruikersvoorkeurenApiService
{
	private readonly HttpClient _httpClient;

	public GebruikersvoorkeurenApiService(HttpClient httpClient)
	{
		ArgumentNullException.ThrowIfNull(httpClient);

		_httpClient = httpClient;
	}

	public Task<GetGebruikersvoorkeurenResponse?> GetGebruikersvoorkeurenAsync()
	{
		return _httpClient.GetFromJsonAsync<GetGebruikersvoorkeurenResponse>("gebruikersvoorkeuren");
	}

	public async Task<UpdateGebruikersvoorkeurenReponse?> UpdateGebruikersvoorkeurenAsync(UpdateGebruikersvoorkeurenRequest request)
	{
		ArgumentNullException.ThrowIfNull(request);

		var response = await _httpClient.PatchAsJsonAsync("gebruikersvoorkeuren", request);

		return await response.Content.ReadFromJsonAsync<UpdateGebruikersvoorkeurenReponse>();
	}
}
