using AdventureArray.Domain.Rides;

namespace AdventureArray.Application.UI.Model.Rides;

public record GetRidesResponse(List<Ride> Rides);

public record Ride(
	int Id,
	string Name,
	RideType Type,
	int Capacity,
	int DurationMinutes,
	int HeightRestrictionInCentimeters,
	string Location,
	int WaitTimeMinutes,
	DateTime WaitTimeUpdated);
