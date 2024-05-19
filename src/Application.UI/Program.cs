using System.Text.Encodings.Web;
using AdventureArray.Application.UI.Client;
using AdventureArray.Application.UI.Infrastructure.ClientNotifications.Extensions;
using AdventureArray.Application.UI.Infrastructure.Identity.Extensions;
using AdventureArray.Application.UI.Model;
using AdventureArray.Infrastructure.ServiceDefaults;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddCustomIdentity();

builder.Services.AddTransient<IAuthenticationSchemeProvider, MockSchemeProvider>();

builder.AddCustomClientNotifications();

builder.Services.AddRazorComponents()
	.AddInteractiveWebAssemblyComponents();

builder.Services.AddMudServices();
builder.Services.AddMapster();

builder.Services.AddValidatorsFromAssembly(typeof(ModelAssemblyLocator).Assembly);

if (!builder.Environment.IsDevelopment())
{
	builder.Services.AddResponseCompression(opts =>
	{
		opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/octet-stream"]);
	});
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
}
else
{
	app.UseResponseCompression();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
	.AddInteractiveWebAssemblyRenderMode()
	.AddAdditionalAssemblies(typeof(ClientAssemblyLocator).Assembly);

app.MapDefaultEndpoints();
app.MapCustomClientNotificationsHub();

app.Run();

sealed class MockSchemeProvider(IOptions<AuthenticationOptions> options) : AuthenticationSchemeProvider(options)
{
	public override Task<AuthenticationScheme?> GetSchemeAsync(string name)
	{
		var scheme = new AuthenticationScheme("IntegrationTest", "IntegrationTest", typeof(MockAuthenticationHandler));
		return Task.FromResult((AuthenticationScheme?)scheme);
	}
}
sealed class MockAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
	: AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
	const string UserIdClaimValue = "userId";
	const string NameClaimValue = "testuser";
	const string EmailClaimValue = "testuser@example.com";

	protected override Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		Claim[] claims =
		[
			new Claim("sub", UserIdClaimValue),
			new Claim("name", NameClaimValue),
			new Claim("email", EmailClaimValue)
		];

		var identity = new ClaimsIdentity(claims, authenticationType: "fake", nameType: "name", roleType: null);
		var principal = new ClaimsPrincipal(identity);

		var ticket = new AuthenticationTicket(principal, "Test");
		return Task.FromResult(AuthenticateResult.Success(ticket));
	}
}
