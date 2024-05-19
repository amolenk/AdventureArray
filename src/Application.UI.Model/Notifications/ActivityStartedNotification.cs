namespace AdventureArray.Application.UI.Model.Notifications;

public record ActivityStartedNotification(Guid Id, string Name, string Description, DateTime Timestamp, float? Progress = null)
	: INotification;

