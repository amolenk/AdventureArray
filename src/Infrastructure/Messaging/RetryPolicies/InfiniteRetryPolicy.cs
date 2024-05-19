namespace AdventureArray.Infrastructure.Messaging.RetryPolicies;

public static class InfiniteRetryPolicy
{
	/// <summary>
	/// Retry policy voor <see cref="InfiniteRetryException"/>.
	/// De retry policy is als volgt:
	/// <para>5 immediate retries</para>
	/// <para>5 retries met een exponential backoff</para>
	/// <para>Een maand aan retries met een interval van 1 minuut</para>
	/// </summary>
	public static void ConfigureInfiniteRetryPolicy(this IRetryConfigurator retryConfigurator)
	{
		retryConfigurator.Handle<InfiniteRetryException>();

		retryConfigurator.Immediate(5);

		retryConfigurator.Exponential(5, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(1));

		// 'infinite' retries zijn niet mogelijk, maar we kunnen wel een hoge retryCount instellen
		// 44.640 minuten zijn 31 dagen
		retryConfigurator.Interval(44_640, TimeSpan.FromMinutes(1));
	}
}

#pragma warning disable RCS1194 // Implement exception constructors
/// <summary>
/// Deze exception wordt gebruikt om aan te geven dat een bericht oneindig vaak opnieuw verwerkt moet worden door mass transit.
/// </summary>
public abstract class InfiniteRetryException : Exception;
#pragma warning restore RCS1194 // Implement exception constructors
