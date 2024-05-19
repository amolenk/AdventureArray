using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace AdventureArray.Application.UI.Client.Infrastructure.Messaging;

/// <summary>
/// Extension methods for <see cref="WebAssemblyHostBuilder"/>.
/// </summary>

[ExcludeFromCodeCoverage]
public static class WebAssemblyHostBuilderExtensions
{
	/// <summary>
	/// Configures MassTransit mediator.
	/// </summary>
	public static void AddCustomMassTransitMediator(this WebAssemblyHostBuilder builder)
	{
		ArgumentNullException.ThrowIfNull(builder);

		builder.Services.AddMediator();

		// Simplifies testing.
		builder.Services.AddSingleton<IMediatorSubscriber, MediatorSubscriber>();

		// Use a custom process ID provider that works in WebAssembly.
		NewId.SetProcessIdProvider(new WebAssemblyProcessIdProvider());
	}
}
