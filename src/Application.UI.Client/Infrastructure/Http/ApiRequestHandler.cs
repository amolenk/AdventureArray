using System.Diagnostics.CodeAnalysis;

namespace AdventureArray.Application.UI.Client.Infrastructure.Http;

/// <summary>
/// Handler that ensures the HTTP response is successful, throwing an exception otherwise.
/// Also supports simulating a delay and failure rate for testing purposes.
/// </summary>
public class ApiRequestHandler : DelegatingHandler
{
	public static class DeveloperSettings
	{
		public static int DelayInMilliseconds { get; set; }
		public static int FailureRate { get; set; }
	}

	private static readonly Random Random = new();

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		await ApplyDevTooling(cancellationToken);

		// Call the inner handler, which will send the request down the pipeline.
		var response = await base.SendAsync(request, cancellationToken);

		if (response.IsSuccessStatusCode) return response;

		try
		{
			var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken);
			if (problemDetails is not null)
			{
				throw new ApiRequestException(problemDetails);
			}
		}
		catch (System.Text.Json.JsonException)
		{
			// Ignore the exception and continue with the default behavior.
		}

		throw new ApiRequestException(new ProblemDetails()
		{
			Title = "Er is een fout opgetreden bij het aanroepen van de API.",
			Detail = await response.Content.ReadAsStringAsync(cancellationToken),
			Status = (int)response.StatusCode
		});
	}

	[ExcludeFromCodeCoverage(Justification = "Developer tool for testing purposes.")]
	private static async Task ApplyDevTooling(CancellationToken cancellationToken)
	{
		// Do the delay first, so that simulated failures are delayed as well.
		if (DeveloperSettings.DelayInMilliseconds > 0)
		{
			await Task.Delay(DeveloperSettings.DelayInMilliseconds, cancellationToken);
		}

		if (DeveloperSettings.FailureRate > 0 && Random.Next(100) <= DeveloperSettings.FailureRate)
		{
			throw new ApiRequestException(new ProblemDetails
			{
				Title = "Er is een fout opgetreden bij het aanroepen van de API.",
				Detail = "Het verzoek is mislukt vanwege een gesimuleerde fout.",
				Status = 500
			});
		}
	}
}
