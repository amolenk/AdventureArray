namespace AdventureArray.Application.UI.Model.Notifications;

public record ActivityCompletedNotification(Guid Id, string Name, string Description, DateTime Timestamp)
	: INotification;
