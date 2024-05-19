namespace AdventureArray.Application.UI.Infrastructure.ClientNotifications.Extensions;

public static class WebApplicationExtensions
{
	public static WebApplication MapCustomClientNotificationsHub(this WebApplication app)
	{
		app.MapHub<ClientNotificationHub>("/hub").RequireAuthorization();

		return app;
	}
}
