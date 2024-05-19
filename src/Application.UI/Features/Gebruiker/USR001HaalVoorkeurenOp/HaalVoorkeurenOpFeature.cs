using AdventureArray.Application.UI.Client.Infrastructure.Identity;
using AdventureArray.Application.UI.Model.Gebruiker;
using AdventureArray.Domain.Gebruiker;

namespace AdventureArray.Application.UI.Features.Gebruiker.USR001HaalVoorkeurenOp;

public class ApiEndpoint : IApiEndpoint
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapGet("gebruikersvoorkeuren", async (
				ClaimsPrincipal principal,
				IScopedMediator mediator) =>
			{
				var query = new GetGebruikersvoorkeurenQuery(UserInfo.FromClaimsPrincipal(principal).UserId);

				var result = await mediator.Send<GetGebruikersvoorkeurenQuery, GetGebruikersvoorkeurenResponse>(query);

				return Results.Ok(result);
			})
			.WithName("GetGebruikersvoorkeuren")
			.Produces<GetGebruikersvoorkeurenResponse>();
	}
}

public record GetGebruikersvoorkeurenQuery(string GebruikerId);

public class GetVoorkeurenQueryHandler : IConsumer<GetGebruikersvoorkeurenQuery>
{
	private readonly ApplicationDbContext _dbContext;

	public GetVoorkeurenQueryHandler(ApplicationDbContext dbContext)
	{
		ArgumentNullException.ThrowIfNull(dbContext);

		_dbContext = dbContext;
	}

	public async Task Consume(ConsumeContext<GetGebruikersvoorkeurenQuery> context)
	{
		var message = context.Message;

		var gebruikersvoorkeuren = await _dbContext.Gebruikersvoorkeuren.FindAsync([message.GebruikerId], cancellationToken: context.CancellationToken)
								   ?? new Gebruikersvoorkeuren(message.GebruikerId);

		await context.RespondAsync(gebruikersvoorkeuren.Adapt<GetGebruikersvoorkeurenResponse>());
	}
}
