using System.Collections.ObjectModel;
using AdventureArray.Application.UI.Client.Infrastructure.Messaging;
using AdventureArray.Application.UI.Model.General;
using AdventureArray.Application.UI.Model.Notifications;

namespace AdventureArray.Application.UI.Client.Infrastructure.Berichten;

public interface IBerichtenManager
{
	public ObservableCollection<Bericht> Berichten { get; }
}

public sealed class BerichtenManager :
	IBerichtenManager,
	IConsumer<ActivityStartedNotification>,
	IConsumer<ActivityProgressChangedNotification>,
	IConsumer<ActivityCompletedNotification>,
	IDisposable
{
	private readonly IMediator _mediator;
	private readonly IDisposable? _mediatorSubscription;

	public ObservableCollection<Bericht> Berichten { get; }

	public BerichtenManager(IMediator mediator, IMediatorSubscriber mediatorSubscriber)
	{
		ArgumentNullException.ThrowIfNull(mediator);
		ArgumentNullException.ThrowIfNull(mediatorSubscriber);

		_mediator = mediator;
		_mediatorSubscription = mediatorSubscriber.Subscribe(this);

		Berichten = new ObservableCollection<Bericht>();
	}

	public Task Consume(ConsumeContext<ActivityStartedNotification> context)
	{
		var message = context.Message;

		var bericht = new Bericht
		{
			Id = message.Id,
			Titel = message.Name,
			Details = message.Description,
			Ernst = Severity.Info,
			Tijdstip = message.Timestamp
		};

		if (message.Progress is not null)
		{
			bericht.Voortgang = message.Progress.Value;
		}

		Berichten.Add(bericht);

		return Task.CompletedTask;
	}

	public Task Consume(ConsumeContext<ActivityProgressChangedNotification> context)
	{
		var message = context.Message;

		var bericht = Berichten.FirstOrDefault(m => m.Id == message.Id);
		if (bericht is null) return Task.CompletedTask;

		bericht.Voortgang = message.Progress;
		bericht.Tijdstip = message.Timestamp;

		return Task.CompletedTask;
	}

	public async Task Consume(ConsumeContext<ActivityCompletedNotification> context)
	{
		var message = context.Message;

		var bericht = Berichten.FirstOrDefault(m => m.Id == message.Id);
		if (bericht is null) return;

		bericht.Titel = message.Name;
		bericht.Details = message.Description;
		bericht.Tijdstip = DateTime.UtcNow;
		bericht.Ernst = Severity.Success;
		bericht.IsAfgerond = true;

		await _mediator.Publish(new UserNotification(bericht.Titel));
	}

	public void Dispose()
	{
		_mediatorSubscription?.Dispose();
	}
}
