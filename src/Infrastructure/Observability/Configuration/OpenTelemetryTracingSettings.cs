namespace AdventureArray.Infrastructure.Observability.Configuration;

/// <summary>
/// Provides the client configuration settings for configuring Open Telemetry tracing.
/// </summary>
public sealed class OpenTelemetryTracingSettings
{
	/// <summary>
	/// Whether or not to export traces to the console.
	/// </summary>
	public bool ExportToConsole { get; set; }

	/// <summary>
	/// The settings for OpenTelemetryProtocol.
	/// </summary>
	public OpenTelemetryProtocolSettings? OpenTelemetryProtocol { get; set; }
}
