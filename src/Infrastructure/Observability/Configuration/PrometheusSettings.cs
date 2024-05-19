namespace AdventureArray.Infrastructure.Observability.Configuration;

/// <summary>
/// Provides the client configuration settings for configuring Prometheus.
/// </summary>
public sealed class PrometheusSettings
{
	/// <summary>
	/// Whether or not to expose a scraping endpoint for Prometheus.
	/// </summary>
	public bool ExposeScrapingEndpoint { get; set; }
}
