using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace AdventureArray.Application.UI.Infrastructure.Identity.Extensions;

/// <summary>
/// Extension methods for <see cref="IHostApplicationBuilder"/>.
/// </summary>
public static class HostApplicationBuilderExtensions
{
	[ExcludeFromCodeCoverage(Justification = "Identity extension methods")]
	public static IHostApplicationBuilder AddCustomIdentity(this IHostApplicationBuilder builder)
	{
		ArgumentNullException.ThrowIfNull(builder);

		const string cookieScheme = CookieAuthenticationDefaults.AuthenticationScheme;
		const string oidcScheme = OpenIdConnectDefaults.AuthenticationScheme;

		// Add services to the container.
		builder.Services.AddAuthentication(oidcScheme)
			.AddOpenIdConnect(oidcScheme, oidcOptions =>
			{
				builder.Configuration.Bind("OpenIdConnect", oidcOptions);

				// The OIDC handler must use a sign-in scheme capable of persisting
				// user credentials across requests.
				oidcOptions.SignInScheme = cookieScheme;

				// The "openid" and "profile" scopes are required for the OIDC handler
				// and included by default. You should enable these scopes here if scopes
				// are provided by configuration because configuration may overwrite the scopes collection.
				// The "offline_access" scope is required for the refresh token.
				oidcOptions.Scope.Add(OpenIdConnectScope.OpenIdProfile);
				oidcOptions.Scope.Add(OpenIdConnectScope.OfflineAccess);

				// SaveTokens is set to true to save the access and refresh tokens in the
				// cookie, so the app can authenticate requests for the API.
				oidcOptions.SaveTokens = true;

				// The following paths must match the redirect and post logout redirect
				// paths configured when registering the application with the OIDC provider.
				// For Microsoft Entra ID, this is accomplished through the "Authentication"
				// blade of the application's registration in the Azure portal. Both the
				// signin and signout paths must be registered as Redirect URIs. The default
				// values are "/signin-oidc" and "/signout-callback-oidc".
				// Microsoft Identity currently only redirects back to the
				// SignedOutCallbackPath if authority is
				// https://login.microsoftonline.com/{TENANT ID}/v2.0/.
				// You can use the "common" authority instead, and logout redirects back to
				// the Blazor app. For more information, see
				// https://github.com/AzureAD/microsoft-authentication-library-for-js/issues/5783
				oidcOptions.CallbackPath = new PathString("/signin-oidc");
				oidcOptions.SignedOutCallbackPath = new PathString("/signout-callback-oidc");

				// The RemoteSignOutPath is the "Front-channel logout URL" for remote single
				// sign-out. The default value is "/signout-oidc".
				oidcOptions.RemoteSignOutPath = new PathString("/signout-oidc");

				// Only require HTTPS in production.
				if (builder.Environment.IsDevelopment())
				{
					oidcOptions.RequireHttpsMetadata = false;
				}

				// Setting ResponseType to "code" configures the OIDC handler to use
				// authorization code flow. Implicit grants and hybrid flows are unnecessary
				// in this mode. In a Microsoft Entra ID app registration, you don't need to
				// select either box for the authorization endpoint to return access tokens
				// or ID tokens. The OIDC handler automatically requests the appropriate
				// tokens using the code returned from the authorization endpoint.
				oidcOptions.ResponseType = OpenIdConnectResponseType.Code;

				// Many OIDC servers use "name" and "role" rather than the SOAP/WS-Fed
				// defaults in ClaimTypes. If you don't use ClaimTypes, mapping inbound
				// claims to ASP.NET Core's ClaimTypes isn't necessary.
				oidcOptions.MapInboundClaims = false;
				oidcOptions.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
				oidcOptions.TokenValidationParameters.RoleClaimType = "role";

				// Many OIDC providers work with the default issuer validator, but the
				// configuration must account for the issuer parameterized with "{TENANT ID}"
				// returned by the "common" endpoint's /.well-known/openid-configuration
				// For more information, see
				// https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/issues/1731
				//var microsoftIssuerValidator = AadIssuerValidator.GetAadIssuerValidator(oidcOptions.Authority);
				//oidcOptions.TokenValidationParameters.IssuerValidator = microsoftIssuerValidator.Validate;
			})
			.AddCookie(cookieScheme);

		// Attach a cookie OnValidatePrincipal callback to get a new access token when the current one expires, and
		// reissue a cookie with the new access token saved inside. If the refresh fails, the user will be signed out.
		builder.Services.AddSingleton<CookieOidcRefresher>();

		builder.Services.AddOptions<CookieAuthenticationOptions>(cookieScheme).Configure<CookieOidcRefresher>(
			(cookieOptions, refresher) =>
			{
				cookieOptions.Events.OnValidatePrincipal = new Func<CookieValidatePrincipalContext, Task>(context =>
					refresher.ValidateOrRefreshCookieAsync(context, oidcScheme));
			});
		builder.Services.AddOptions<OpenIdConnectOptions>(oidcScheme).Configure(oidcOptions =>
		{
			// Request a refresh_token.
			oidcOptions.Scope.Add("offline_access");
			// Store the refresh_token.
			oidcOptions.SaveTokens = true;
		});

		builder.Services.AddAuthorization();

		builder.Services.AddScoped<AuthenticationStateProvider, PersistingAuthenticationStateProvider>();

		return builder;
	}
}
