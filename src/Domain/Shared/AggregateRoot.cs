namespace AdventureArray.Domain.Shared;

// Marker interface
public interface IAggregateRoot
{
	void AddDomainEvent(IDomainEvent domainEvent);
	void ClearDomainEvents();
	IEnumerable<IDomainEvent> GetPendingEvents();
	bool HasPendingEvents();
}

// TODO IEquatable toevoegen
public abstract class AggregateRoot<TKey> : IAggregateRoot
{
	private readonly List<IDomainEvent> _domainEvents = [];

	protected abstract TKey Key { get; }

	public void AddDomainEvent(IDomainEvent domainEvent)
	{
		_domainEvents.Add(domainEvent);
	}

	public bool HasPendingEvents() => _domainEvents.Count != 0;

	public IEnumerable<IDomainEvent> GetPendingEvents() => _domainEvents;

	public void ClearDomainEvents()
	{
		_domainEvents.Clear();
	}
}
