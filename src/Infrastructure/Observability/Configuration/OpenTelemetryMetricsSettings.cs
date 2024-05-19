namespace AdventureArray.Infrastructure.Observability.Configuration;

/// <summary>
/// Provides the client configuration settings for configuring Open Telemetry metrics.
/// </summary>
public sealed class OpenTelemetryMetricsSettings
{
	/// <summary>
	/// Whether or not to export traces to the console.
	/// </summary>
	public bool ExportToConsole { get; set; }

	/// <summary>
	/// The settings for Prometheus.
	/// </summary>
	public PrometheusSettings? Prometheus { get; set; }
}
