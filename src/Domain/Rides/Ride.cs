namespace AdventureArray.Domain.Rides;

public class Ride : AggregateRoot<int>, IConcurrentEntity
{
	public int Id { get; }

	/// <summary>
	/// Name of the ride.
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Type of ride (e.g., roller coaster, water ride).
	/// </summary>
	public RideType Type { get; set; }

	/// <summary>
	/// Maximum number of riders per ride cycle.
	/// </summary>
	public int Capacity { get; set; }

	/// <summary>
	/// Duration of the ride in minutes.
	/// </summary>
	public int DurationMinutes { get; set; }

	/// <summary>
	/// Minimum height requirement in centimeters.
	/// </summary>
	public int HeightRestrictionInCentimeters { get; set; }

	/// <summary>
	/// Location of the ride within the theme park.
	/// </summary>
	public string Location { get; set; }

	/// <summary>
	/// The total wait time in minutes.
	/// </summary>
	public int WaitTimeMinutes { get; set; }

	/// <summary>
	/// The last time the wait time was updated.
	/// </summary>
	public DateTime WaitTimeUpdated { get; set; }

	public long Versie { get; set; }

	protected override int Key => Id;

	public Ride(int id, string name, RideType type, int capacity, int durationMinutes,
		int heightRestrictionInCentimeters, string location)
	{
		Id = id;
		Name = name;
		Type = type;
		Capacity = capacity;
		DurationMinutes = durationMinutes;
		HeightRestrictionInCentimeters = heightRestrictionInCentimeters;
		Location = location;
	}
}

