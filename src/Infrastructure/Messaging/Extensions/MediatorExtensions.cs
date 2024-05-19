namespace AdventureArray.Infrastructure.Messaging.Extensions;

/// <summary>
/// Provides extra functionality for the MassTransit mediator.
/// </summary>
public static class MediatorExtensions
{
	/// <summary>
	/// Extension method to make it easier to send a message that expects a response.
	/// </summary>
	public static async Task<TResult> Send<TMessage, TResult>(this IMediator mediator, TMessage message, CancellationToken cancellationToken = default)
		where TMessage : class
		where TResult : class
	{
		ArgumentNullException.ThrowIfNull(mediator);
		ArgumentNullException.ThrowIfNull(message);

		var client = mediator.CreateRequestClient<TMessage>(RequestTimeout.None);

		var response = await client.GetResponse<TResult>(message, cancellationToken);
		return response.Message;
	}
}
