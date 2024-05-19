namespace AdventureArray.Application.UI.Client.Infrastructure.Http;

/// <summary>
/// Thrown when an API request fails.
/// </summary>
#pragma warning disable RCS1194 // Implement exception constructors
public class ApiRequestException(ProblemDetails problemDetails) : Exception(problemDetails.Title)
#pragma warning restore RCS1194 // Implement exception constructors
{
	public ProblemDetails ProblemDetails { get; } = problemDetails;
}
