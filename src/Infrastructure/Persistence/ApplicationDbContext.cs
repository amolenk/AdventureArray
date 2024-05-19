using AdventureArray.Domain.Algemeen;
using AdventureArray.Domain.Gebruiker;
using AdventureArray.Domain.Rides;

namespace AdventureArray.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
	public DbSet<Ride> Rides => Set<Ride>();
	public DbSet<RideWaitTime> RideWaitTimes => Set<RideWaitTime>();
	public DbSet<Gebruikersvoorkeuren> Gebruikersvoorkeuren => Set<Gebruikersvoorkeuren>();
	public DbSet<Proces> Processen => Set<Proces>();

	private readonly IMediator _mediator;

	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IMediator mediator)
		: base(options)
	{
		ArgumentNullException.ThrowIfNull(mediator);

		_mediator = mediator;
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasAnnotation(CustomDataAnnotations.EnsureCitusExtension, true);

		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

		// Add MassTransit outbox
		modelBuilder.AddInboxStateEntity();
		modelBuilder.AddOutboxMessageEntity();
		modelBuilder.AddOutboxStateEntity();
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
	{
		var entitiesWithPendingEvents = ChangeTracker
			.Entries<IAggregateRoot>()
			.Where(e => e.Entity.HasPendingEvents())
			.Select(e => e.Entity)
			.ToList();

		foreach (var entity in ChangeTracker.Entries<IConcurrentEntity>())
		{
			if (entity.State == EntityState.Unchanged)
			{
				continue;
			}

			entity.Entity.Versie++;
		}

		foreach (var entity in ChangeTracker.Entries<IHasCreationTimestamp>())
		{
			if (entity.State == EntityState.Added)
			{
				entity.Entity.AangemaaktOp = DateTime.UtcNow;
			}
		}

		foreach (var entity in entitiesWithPendingEvents)
		{
			await PublishPendingEventsAsync(entity, cancellationToken);
		}

		return await base.SaveChangesAsync(cancellationToken);
	}

	private async Task PublishPendingEventsAsync(IAggregateRoot root, CancellationToken cancellationToken)
	{
		foreach (var domainEvent in root.GetPendingEvents())
		{
			await _mediator.Publish(domainEvent, cancellationToken);
		}

		root.ClearDomainEvents();
	}
}
