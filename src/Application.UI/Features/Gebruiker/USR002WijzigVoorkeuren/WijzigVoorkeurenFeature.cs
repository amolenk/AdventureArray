using AdventureArray.Application.UI.Client.Infrastructure.Identity;
using AdventureArray.Application.UI.Model.Gebruiker;
using AdventureArray.Domain.Gebruiker;
using DarkModeSetting = AdventureArray.Application.UI.Model.Gebruiker.DarkModeSetting;

namespace AdventureArray.Application.UI.Features.Gebruiker.USR002WijzigVoorkeuren;

public class ApiEndpoint : IApiEndpoint
{
	/// <summary>
	/// Adds the routes for this feature to the API.
	/// </summary>
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapPatch("gebruikersvoorkeuren", async (
				ClaimsPrincipal principal,
				UpdateGebruikersvoorkeurenRequest request,
				IScopedMediator mediator) =>
			{
				var command = new UpdateVoorkeurenCommand
				{
					GebruikerId = UserInfo.FromClaimsPrincipal(principal).UserId,
					DarkMode = request.DarkMode
				};

				var response = await mediator.Send<UpdateVoorkeurenCommand, UpdateGebruikersvoorkeurenReponse>(command);

				return Results.Ok(response);
			})
			.WithName("UpdateGebruikersvoorkeuren")
			.Produces<UpdateGebruikersvoorkeurenReponse>();
	}
}

public record UpdateVoorkeurenCommand
{
	public required string GebruikerId { get; init; }

	public DarkModeSetting? DarkMode { get; init; }
}

public class UpdateVoorkeurenHandler : IConsumer<UpdateVoorkeurenCommand>
{
	private readonly ApplicationDbContext _dbContext;

	public UpdateVoorkeurenHandler(ApplicationDbContext dbContext)
	{
		ArgumentNullException.ThrowIfNull(dbContext);

		_dbContext = dbContext;
	}

	public async Task Consume(ConsumeContext<UpdateVoorkeurenCommand> context)
	{
		var message = context.Message;

		var gebruikersvoorkeuren = await _dbContext.Gebruikersvoorkeuren.FindAsync([message.GebruikerId], cancellationToken: context.CancellationToken);
		if (gebruikersvoorkeuren is null)
		{
			gebruikersvoorkeuren = new Gebruikersvoorkeuren(message.GebruikerId);
			_dbContext.Gebruikersvoorkeuren.Add(gebruikersvoorkeuren);
		}

		if (message.DarkMode is not null)
		{
			gebruikersvoorkeuren.SetDarkMode(message.DarkMode.Value.Adapt<Domain.Gebruiker.DarkModeSetting>());
		}

		await _dbContext.SaveChangesAsync(context.CancellationToken);

		var response = (await _dbContext.Gebruikersvoorkeuren.FindAsync([message.GebruikerId], cancellationToken: context.CancellationToken))
			.Adapt<UpdateGebruikersvoorkeurenReponse>();

		await context.RespondAsync(response);
	}
}
