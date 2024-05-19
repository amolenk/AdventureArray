using AdventureArray.Application.UI.Client.Infrastructure.Identity;
using Microsoft.AspNetCore.SignalR;

namespace AdventureArray.Application.UI.Infrastructure.ClientNotifications;

public class HubUserIdProvider : IUserIdProvider
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly ILogger _logger;

	private static readonly Action<ILogger, string, string, Exception?> _logUserConnectedToHub =
		LoggerMessage.Define<string, string>(LogLevel.Information, new EventId(0, "UserConnectedToHub"), "User {UserId} ({UserEmail}) connected to hub.");

	public HubUserIdProvider(IHttpContextAccessor httpContextAccessor, ILogger<HubUserIdProvider> logger)
	{
		ArgumentNullException.ThrowIfNull(httpContextAccessor);
		ArgumentNullException.ThrowIfNull(logger);

		_httpContextAccessor = httpContextAccessor;
		_logger = logger;
	}

	public string? GetUserId(HubConnectionContext connection)
	{
		var principal = (_httpContextAccessor.HttpContext?.User) ?? throw new InvalidOperationException("No user found in the current HttpContext.");
		var userInfo = UserInfo.FromClaimsPrincipal(principal);

		_logUserConnectedToHub(_logger, userInfo.UserId, userInfo.Email, null);

		return userInfo.UserId;
	}
}
