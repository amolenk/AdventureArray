using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;

namespace AdventureArray.Application.UI.Client.Infrastructure.Identity;

/// <summary>
/// This is a client-side AuthenticationStateProvider that determines the user's authentication state by
/// looking for data persisted in the page when it was rendered on the server. This authentication state will
/// be fixed for the lifetime of the WebAssembly application. So, if the user needs to log in or out, a full
/// page reload is required.
///
/// This only provides a user name and email for display purposes. It does not actually include any tokens
/// that authenticate to the server when making subsequent requests. That works separately using a
/// cookie that will be included on HttpClient requests to the server.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "PersistentComponentState is niet goed te mocken/instanti�ren waardoor deze class niet getest kan worden. Ook bevat die maar weinig logica.")]
internal sealed class PersistentAuthenticationStateProvider : AuthenticationStateProvider
{
	private static readonly Task<AuthenticationState> DefaultUnauthenticatedTask =
		Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

	private readonly Task<AuthenticationState> _authenticationStateTask = DefaultUnauthenticatedTask;

	public PersistentAuthenticationStateProvider(PersistentComponentState state)
	{
		// // Check that we've got UserInfo state in the page.
		// if (!state.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var userInfo) || userInfo is null)
		// {
		// 	return;
		// }
		//
		// // Create new authentication state based on the persistent user information.
		// _authenticationStateTask = Task.FromResult(new AuthenticationState(userInfo.ToClaimsPrincipal()));

		_authenticationStateTask = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(
			new ClaimsIdentity("fake", "Sander", "role"))));
	}

	public override Task<AuthenticationState> GetAuthenticationStateAsync() => _authenticationStateTask;
}
