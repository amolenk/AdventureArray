using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace AdventureArray.Application.UI.Infrastructure.Identity;

[ExcludeFromCodeCoverage(Justification = "Auth infra code")]
// https://github.com/dotnet/aspnetcore/issues/8175
internal sealed class CookieOidcRefresher(IOptionsMonitor<OpenIdConnectOptions> oidcOptionsMonitor) : IDisposable
{
	private readonly HttpClient _refreshClient = new();
	private readonly OpenIdConnectProtocolValidator _oidcTokenValidator = new()
	{
		// Refresh requests do not use the nonce parameter. Otherwise, we'd use oidcOptions.ProtocolValidator.
		RequireNonce = false,
	};

	public async Task ValidateOrRefreshCookieAsync(CookieValidatePrincipalContext validateContext, string oidcScheme)
	{
		// Retrieve the expiration time of the access token from the cookie properties.
		// If the expiration time cannot be parsed, exit the method.
		var accessTokenExpirationText = validateContext.Properties.GetTokenValue("expires_at");
		if (!DateTimeOffset.TryParse(accessTokenExpirationText, out var accessTokenExpiration))
		{
			return;
		}

		// Retrieve the OIDC options for the current scheme and get the current time.
		// If the access token is not due to expire within the next 5 minutes, exit the method.
		var oidcOptions = oidcOptionsMonitor.Get(oidcScheme);
		var now = oidcOptions.TimeProvider!.GetUtcNow();
		if (now + TimeSpan.FromMinutes(5) < accessTokenExpiration)
		{
			return;
		}

		// Retrieve the OIDC configuration and the token endpoint from the OIDC options.
		var oidcConfiguration = await oidcOptions.ConfigurationManager!.GetConfigurationAsync(validateContext.HttpContext.RequestAborted);
		var tokenEndpoint = oidcConfiguration.TokenEndpoint ?? throw new InvalidOperationException("Cannot refresh cookie. TokenEndpoint missing!");

		// Send a refresh request to the token endpoint using the refresh token from the cookie properties.
		using var refreshResponse = await _refreshClient.PostAsync(tokenEndpoint,
			new FormUrlEncodedContent(new Dictionary<string, string?>()
			{
				["grant_type"] = "refresh_token",
				["client_id"] = oidcOptions.ClientId,
				["client_secret"] = oidcOptions.ClientSecret,
				["scope"] = string.Join(" ", oidcOptions.Scope),
				["refresh_token"] = validateContext.Properties.GetTokenValue("refresh_token"),
			}));

		// If the refresh request fails, reject the principal and exit the method.
		if (!refreshResponse.IsSuccessStatusCode)
		{
			validateContext.RejectPrincipal();
			return;
		}

		// Parse the refresh response and get the ID token.
		var refreshJson = await refreshResponse.Content.ReadAsStringAsync();
		var message = new OpenIdConnectMessage(refreshJson);

		// Clone the token validation parameters from the OIDC options and set the issuer signing keys.
		var validationParameters = oidcOptions.TokenValidationParameters.Clone();
		if (oidcOptions.ConfigurationManager is BaseConfigurationManager baseConfigurationManager)
		{
			validationParameters.ConfigurationManager = baseConfigurationManager;
		}
		else
		{
			validationParameters.ValidIssuer = oidcConfiguration.Issuer;
			validationParameters.IssuerSigningKeys = oidcConfiguration.SigningKeys;
		}

		// Validate the ID token from the refresh response.
		var validationResult = await oidcOptions.TokenHandler.ValidateTokenAsync(message.IdToken, validationParameters);

		// If the ID token is not valid, reject the principal and exit the method.
		if (!validationResult.IsValid)
		{
			validateContext.RejectPrincipal();
			return;
		}

		// Validate the refresh response.
		_oidcTokenValidator.ValidateTokenResponse(new OpenIdConnectProtocolValidationContext
		{
			ProtocolMessage = message,
			ClientId = oidcOptions.ClientId,

			// For refresh tokens we don't want to validate the Nonce, but if the received token contains a nonce it will be evaluated anyways.
			// However we have no value against which we can validate it in case of a refresh token, so we remove it from the token before validation.
			// In the various OAuth 2.0 flows for obtaining access tokens the nonce parameter is used to prevent replay attacks, especially in flows where there's a redirection involved (e.g., authorization code flow and implicit flow).
			// In contrast, checking the nonce is not typically done for refresh tokens. The client presents the refresh token to the authorization server, which then validates it and issues a new access token.
			// Since there's no redirection or user interaction involved in this flow, the need for a nonce to prevent replay attacks is diminished.
			ValidatedIdToken = GetJwtSecurityTokenWithoutNonce(validationResult.SecurityToken),
		});

		// Replace the principal.
		validateContext.ShouldRenew = true;
		validateContext.ReplacePrincipal(new ClaimsPrincipal(validationResult.ClaimsIdentity));

		// Store the new tokens in the cookie properties.
		var expiresIn = int.Parse(message.ExpiresIn, NumberStyles.Integer, CultureInfo.InvariantCulture);
		var expiresAt = now + TimeSpan.FromSeconds(expiresIn);
		validateContext.Properties.StoreTokens([
		new() { Name = "access_token", Value = message.AccessToken },
			new() { Name = "id_token", Value = message.IdToken },
			new() { Name = "refresh_token", Value = message.RefreshToken },
			new() { Name = "token_type", Value = message.TokenType },
			new() { Name = "expires_at", Value = expiresAt.ToString("o", CultureInfo.InvariantCulture) },
		]);
	}

	public void Dispose() => _refreshClient.Dispose();

	private static JwtSecurityToken GetJwtSecurityTokenWithoutNonce(SecurityToken securityToken)
	{
		var jwtSecurityToken = JwtSecurityTokenConverter.Convert(securityToken as JsonWebToken);
		jwtSecurityToken.Payload.Remove("nonce");
		return jwtSecurityToken;
	}
}
