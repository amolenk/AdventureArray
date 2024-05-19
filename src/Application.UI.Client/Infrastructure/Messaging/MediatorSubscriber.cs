namespace AdventureArray.Application.UI.Client.Infrastructure.Messaging;

/// <summary>
/// Classes that implement this interface can subscribe to the <see cref="IMediator"/>.
/// Added to simplify testing.
/// </summary>
public interface IMediatorSubscriber
{
	IDisposable Subscribe<T>(T instance) where T : class;
}

public class MediatorSubscriber : IMediatorSubscriber
{
	private readonly IMediator _mediator;

	public MediatorSubscriber(IMediator mediator)
	{
		ArgumentNullException.ThrowIfNull(mediator);

		_mediator = mediator;
	}

	public IDisposable Subscribe<T>(T instance) where T : class => _mediator.ConnectInstance(instance);
}

