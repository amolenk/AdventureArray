using System.Security.Claims;

namespace AdventureArray.Application.UI.Client.Infrastructure.Identity;

/// <summary>
/// Add properties to this class and update the server and client AuthenticationStateProviders
/// to expose more information about the authenticated user to the client.
/// </summary>
public sealed class UserInfo
{
	public required string UserId { get; init; }
	public required string Name { get; init; }
	public required string Email { get; init; }

	public const string UserIdClaimType = "sub";
	public const string NameClaimType = "name";
	public const string EmailClaimType = "email";

	public static UserInfo FromClaimsPrincipal(ClaimsPrincipal principal) =>
		new()
		{
			UserId = GetRequiredClaim(principal, UserIdClaimType),
			Name = GetRequiredClaim(principal, NameClaimType),
			Email = GetRequiredClaim(principal, EmailClaimType)
		};

	public ClaimsPrincipal ToClaimsPrincipal()
	{
		Claim[] claims =
		[
			new Claim(UserIdClaimType, UserId),
			new Claim(NameClaimType, Name),
			new Claim(EmailClaimType, Email)
		];

		var identity = new ClaimsIdentity(claims, authenticationType: nameof(UserInfo), nameType: NameClaimType, roleType: null);

		return new ClaimsPrincipal(identity);
	}

	private static string GetRequiredClaim(ClaimsPrincipal principal, string claimType) =>
		principal.FindFirst(claimType)?.Value ?? throw new InvalidOperationException($"Could not find required '{claimType}' claim.");
}
