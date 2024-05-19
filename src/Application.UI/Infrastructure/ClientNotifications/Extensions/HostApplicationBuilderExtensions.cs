using Microsoft.AspNetCore.SignalR;

namespace AdventureArray.Application.UI.Infrastructure.ClientNotifications.Extensions;

/// <summary>
/// Extension methods for <see cref="IHostApplicationBuilder"/>.
/// </summary>
public static class HostApplicationBuilderExtensions
{
	public static IHostApplicationBuilder AddCustomClientNotifications(this IHostApplicationBuilder builder)
	{
		ArgumentNullException.ThrowIfNull(builder);

		builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
		builder.Services.AddScoped<IClientNotificationService, ClientNotificationService>();
		builder.Services.AddSingleton<IUserIdProvider, HubUserIdProvider>();
		builder.Services.AddSignalR();

		return builder;
	}
}
