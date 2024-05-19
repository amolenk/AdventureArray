namespace AdventureArray.Infrastructure.Observability.Configuration;

/// <summary>
/// Provides the client configuration settings for configuring Open Telemetry for Azure Monitor.
/// </summary>
public sealed class OpenTelemetryAzureMonitorSettings
{
	/// <summary>
	/// The connection string for Application Insights.
	/// </summary>
	public string ApplicationInsightsConnectionString { get; set; } = string.Empty;
}
