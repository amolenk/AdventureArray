using Microsoft.AspNetCore.Routing;

namespace AdventureArray.Infrastructure.Routing;

/// <summary>
/// Features that implement this interface provide API endpoints.
/// </summary>
public interface IApiEndpoint
{
	/// <summary>
	/// Called to add API endpoints to the application.
	/// </summary>
	void AddRoutes(IEndpointRouteBuilder app);
}
