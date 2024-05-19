namespace AdventureArray.Infrastructure.Observability.Configuration;

public interface IMetricMeters
{
	void AddMeter(string meter);
}

public class MetricMeterManager : IMetricMeters
{
	public static readonly MetricMeterManager Instance = new MetricMeterManager();

	private readonly List<string> _meters;

	private MetricMeterManager()
	{
		_meters = new List<string>();
	}

	public IEnumerable<string> Meters => _meters;

	public void AddMeter(string meter)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(meter);

		_meters.Add(meter);
	}
}
