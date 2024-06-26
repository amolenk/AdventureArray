namespace AdventureArray.Infrastructure.AppHost.Extensions.Kafka;

public class KafkaTopicAnnotation : IResourceAnnotation
{
	public required string TopicName { get; init; }

	public required int NumPartitions { get; init; }
}
