using ProtoBuf;

namespace AdventureArray.Application.Shared.Messages;

[ProtoContract]
public class WaitTimeMessage
{
	[ProtoMember(1)]
	public required int RideId { get; init; }

	[ProtoMember(2)]
	public required int WaitTimeMinutes { get; init; }

	[ProtoMember(3)]
	public required DateTime Timestamp { get; init; }
}


