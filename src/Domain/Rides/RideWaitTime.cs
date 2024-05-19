namespace AdventureArray.Domain.Rides;

public class RideWaitTime : IConcurrentEntity
{
	public long Id { get; set; }

	public int RideId { get; set; }

	public int WaitTimeMinutes { get; set; }

	public DateTime LastUpdated { get; set; }

	public long Versie { get; set; }
}
