namespace AdventureArray.Application.UI.Client.Utilities;

public static class AsyncRequestUtility
{
	public static async Task ExecuteWithLoadingFlagAsync(Func<Task> operation, Action<Boolean> onLoadingStateChanged,
		Boolean bypassDelay = false)
	{
		var delayTask = bypassDelay ? Task.CompletedTask : Task.Delay(TimeSpan.FromSeconds(1));
		var operationTask = operation();

		await Task.WhenAny(delayTask, operationTask);

		if (operationTask.IsCompleted) return;

		onLoadingStateChanged(true);

		try
		{
			await operationTask;
		}
		finally
		{
			onLoadingStateChanged(false);
		}
	}
}
