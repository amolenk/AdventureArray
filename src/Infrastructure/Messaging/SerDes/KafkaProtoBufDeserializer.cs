using Confluent.Kafka;
using ProtoBuf;
using SerializationContext = Confluent.Kafka.SerializationContext;

namespace AdventureArray.Infrastructure.Messaging.SerDes;

public class KafkaProtoBufDeserializer<T> : IDeserializer<T>
{
	public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
	{
		if (isNull) return default!;

		using var ms = new MemoryStream(data.ToArray());
		return Serializer.Deserialize<T>(ms);
	}
}
