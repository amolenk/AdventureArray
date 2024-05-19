namespace AdventureArray.Infrastructure.Observability.Configuration;

/// <summary>
/// Provides the client configuration settings for configuring Open Telemetry.
/// </summary>
public sealed class OpenTelemetrySettings
{
	public const string ConfigurationSectionName = "OpenTelemetry";

	/// <summary>
	/// The configuration settings for Azure Monitor.
	/// </summary>
	public OpenTelemetryAzureMonitorSettings? AzureMonitor { get; set; }

	/// <summary>
	/// The configuration settings for logs.
	/// </summary>
	public OpenTelemetryLoggingSettings Logging { get; set; } = new();

	/// <summary>
	/// The configuration settings for metrics.
	/// </summary>
	public OpenTelemetryMetricsSettings Metrics { get; set; } = new();

	/// <summary>
	/// The configuration settings for tracing.
	/// </summary>
	public OpenTelemetryTracingSettings Tracing { get; set; } = new();
}
