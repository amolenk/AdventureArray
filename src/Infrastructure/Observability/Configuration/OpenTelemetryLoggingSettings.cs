namespace AdventureArray.Infrastructure.Observability.Configuration;

/// <summary>
/// Provides the client configuration settings for configuring Open Telemetry logging.
/// </summary>
public sealed class OpenTelemetryLoggingSettings
{
	/// <summary>
	/// Whether or not to write logs to the console.
	/// </summary>
	public bool ExportToConsole { get; set; } = true;

	/// <summary>
	/// The settings for OpenTelemetryProtocol.
	/// </summary>
	public OpenTelemetryProtocolSettings? OpenTelemetryProtocol { get; set; }
}
