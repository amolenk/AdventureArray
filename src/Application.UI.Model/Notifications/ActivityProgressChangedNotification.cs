namespace AdventureArray.Application.UI.Model.Notifications;

public record ActivityProgressChangedNotification(Guid Id, float Progress, DateTime Timestamp)
	: INotification;

