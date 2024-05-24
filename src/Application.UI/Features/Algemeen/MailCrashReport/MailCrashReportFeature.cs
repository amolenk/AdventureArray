using Dapr.Client;

namespace AdventureArray.Application.UI.Features.Algemeen.MailCrashReport;

public class MailCrashReportApiEndpoint : IApiEndpoint
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapPost("crashreport", async (DaprClient daprClient) =>
			{
				var body = "Hello, world!";
				var metadata = new Dictionary<string, string>
				{
					["emailFrom"] = "getfromsettings@example.com",
					["emailTo"] = "getfromsettings@example.com",
					["subject"] = "Crash report"
				};
				await daprClient.InvokeBindingAsync("sendmail", "create", body, metadata);

				return Results.Ok();
			})
			.WithName("MailCrashReport");
	}
}
