using AdventureArray.Application.UI.Client.Infrastructure.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;

namespace AdventureArray.Application.UI.Infrastructure.Identity;

/// <summary>
/// This is a server-side AuthenticationStateProvider that uses PersistentComponentState to flow the
/// authentication state to the client which is then fixed for the lifetime of the WebAssembly application.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "PersistentComponentState is niet goed te mocken/instanti�ren waardoor deze class niet getest kan worden. Ook bevat die maar weinig logica.")]
public sealed class PersistingAuthenticationStateProvider : AuthenticationStateProvider, IHostEnvironmentAuthenticationStateProvider, IDisposable
{
	private readonly PersistentComponentState _persistentComponentState;
	private readonly PersistingComponentStateSubscription _subscription;
	private Task<AuthenticationState>? _authenticationStateTask;

	public PersistingAuthenticationStateProvider(PersistentComponentState state)
	{
		ArgumentNullException.ThrowIfNull(state);

		_persistentComponentState = state;
		_subscription = state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
	}

	public override Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		if (_authenticationStateTask is null)
		{
			throw new InvalidOperationException(
				$"Do not call {nameof(GetAuthenticationStateAsync)} outside of the DI scope for a Razor component. Typically, this means you can call it only within a Razor component or inside another DI service that is resolved for a Razor component.");
		}

		return _authenticationStateTask;
	}

	public void SetAuthenticationState(Task<AuthenticationState> authenticationStateTask)
	{
		_authenticationStateTask = authenticationStateTask;
	}

	private async Task OnPersistingAsync()
	{
		var authenticationState = await GetAuthenticationStateAsync();
		var principal = authenticationState.User;

		if (principal.Identity?.IsAuthenticated == true)
		{
			_persistentComponentState.PersistAsJson(nameof(UserInfo), UserInfo.FromClaimsPrincipal(principal));
		}
	}

	public void Dispose()
	{
		_subscription.Dispose();
	}
}
