using Confluent.Kafka;
using ProtoBuf;
using SerializationContext = Confluent.Kafka.SerializationContext;

namespace AdventureArray.Infrastructure.Messaging.SerDes;

public class KafkaProtoBufSerializer<T> : ISerializer<T>
{
	public byte[] Serialize(T data, SerializationContext context)
	{
		using var ms = new MemoryStream();
		Serializer.Serialize(ms, data);
		return ms.ToArray();
	}
}
