using System.Diagnostics;
using AdventureArray.Application.UI.Client.Infrastructure.SignalR;
using AdventureArray.Application.UI.Model;
using AdventureArray.Infrastructure.Observability;
using Microsoft.AspNetCore.SignalR;

namespace AdventureArray.Application.UI.Infrastructure.ClientNotifications;

public interface IClientNotificationService
{
	Task PublishNotificationAsync<TNotification>(TNotification notification, string userId)
		where TNotification : INotification;
}

public class ClientNotificationService : IClientNotificationService
{
	private readonly IHubContext<ClientNotificationHub> _hubContext;

	public ClientNotificationService(IHubContext<ClientNotificationHub> hubContext)
	{
		ArgumentNullException.ThrowIfNull(hubContext);

		_hubContext = hubContext;
	}

	public async Task PublishNotificationAsync<TNotification>(TNotification notification, string userId)
		where TNotification : INotification
	{
		ArgumentNullException.ThrowIfNull(notification);

		// Allow the caller to either specify the message type or infer it from the message instance.
		var messageType = typeof(TNotification) == typeof(object) ? notification.GetType() : typeof(TNotification);
		var messageTypeName = messageType.AssemblyQualifiedName;
		var messagePayload = JsonSerializer.Serialize(notification);

		using var activity = ApplicationActivitySource.Source.StartActivity(
			$"{nameof(ClientNotificationService)} publish", ActivityKind.Producer);

		if (activity?.IsAllDataRequested is true)
		{
			activity.AddTag("messageTypeName", messageTypeName);
			activity.AddTag("messagePayload", messagePayload);
			activity.AddTag("userId", userId);
		}

		await _hubContext.Clients.User(userId).SendAsync(
			NotificationHubRelay.OperationName,
			messageTypeName,
			messagePayload);
	}
}
