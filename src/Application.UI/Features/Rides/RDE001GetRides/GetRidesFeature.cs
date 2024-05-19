using AdventureArray.Application.UI.Model.Rides;

namespace AdventureArray.Application.UI.Features.Rides.RDE001GetRides;

public class GetRidesApi : IApiEndpoint
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapGet("rides", async (IScopedMediator mediator) =>
			{
				var query = new GetRidesQuery();
				var result = await mediator.Send<GetRidesQuery, GetRidesResponse>(query);
				return Results.Ok(result);
			})
			.WithName("GetRides")
			.Produces<GetRidesResponse>();
	}
}

public record GetRidesQuery;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class GetRidesQueryHandler : IConsumer<GetRidesQuery>
{
	private readonly ApplicationDbContext _dbContext;

	public GetRidesQueryHandler(ApplicationDbContext dbContext)
	{
		ArgumentNullException.ThrowIfNull(dbContext);

		_dbContext = dbContext;
	}

	public async Task Consume(ConsumeContext<GetRidesQuery> context)
	{
		var processen = await _dbContext.Rides
			.OrderBy(r => r.Name)
			.ToListAsync(context.CancellationToken);

		var response = new GetRidesResponse(processen
			.Select(r => new Ride(r.Id, r.Name, r.Type, r.Capacity, r.DurationMinutes,
				r.HeightRestrictionInCentimeters, r.Location, r.WaitTimeMinutes, r.WaitTimeUpdated))
			.ToList());

		await context.RespondAsync(response);
	}
}
