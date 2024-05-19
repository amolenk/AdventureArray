namespace AdventureArray.Application.UI.Client.Infrastructure.Messaging;

/// <summary>
/// Replacement for the default MassTransit process ID provider that works in WebAssembly.
/// </summary>
public class WebAssemblyProcessIdProvider : IProcessIdProvider
{
	/// <summary>
	/// Use a constant process ID for WebAssembly, as it doesn't have a real process ID.
	/// </summary>
	private static readonly byte[] ProcessId = "wasm"u8.ToArray();

	public byte[] GetProcessId() => ProcessId;
}
