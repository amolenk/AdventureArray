namespace AdventureArray.Infrastructure.Observability.Configuration;

/// <summary>
/// Provides the client configuration settings for configuring Open Telemetry Protocol.
/// </summary>
public sealed class OpenTelemetryProtocolSettings
{
	/// <summary>
	/// The endpoint of the Zipkin server to send traces to.
	/// </summary>
	public string? Endpoint { get; set; }
}
