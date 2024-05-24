using AdventureArray.Infrastructure.Routing;

namespace AdventureArray.Application.RideService.Features.Diagnostics.Ping;

public class PingApiEndpoint : IApiEndpoint
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapGet("ping", () =>
				Results.Ok($"Hello from {Environment.GetEnvironmentVariable("ASPNETCORE_URLS")}!"))
			.WithName("Ping")
			.Produces<string>();
	}
}
