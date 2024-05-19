using AdventureArray.Application.UI.Model.Algemeen;

namespace AdventureArray.Application.UI.Features.Algemeen.ALG001HaalProcessenOp;

public class HaalProcessenOpApiEndpoint : IApiEndpoint
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapGet("processen", async (IScopedMediator mediator) =>
			{
				var query = new HaalProcessenOpQuery();
				var result = await mediator.Send<HaalProcessenOpQuery, HaalProcessenOpResponse>(query);
				return Results.Ok(result);
			})
			.WithName("HaalProcessenOp")
			.Produces<HaalProcessenOpResponse>();
	}
}

public record HaalProcessenOpQuery;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class HaalProcessenOpQueryHandler : IConsumer<HaalProcessenOpQuery>
{
	private readonly ApplicationDbContext _dbContext;

	public HaalProcessenOpQueryHandler(ApplicationDbContext dbContext)
	{
		ArgumentNullException.ThrowIfNull(dbContext);

		_dbContext = dbContext;
	}

	public async Task Consume(ConsumeContext<HaalProcessenOpQuery> context)
	{
		var processen = await _dbContext.Processen
			.OrderBy(p => p.Naam)
			.ToListAsync(context.CancellationToken);

		var response = new HaalProcessenOpResponse(processen
			.Select(p => new Proces(p.Id, p.Naam, p.Voortgang?.Waarde, p.Status, p.Tijdstip, p.ExtraInformatie))
			.ToList());

		await context.RespondAsync(response);
	}
}
