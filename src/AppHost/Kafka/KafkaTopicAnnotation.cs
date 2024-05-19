namespace AdventureArray.Infrastructure.AppHost.Kafka;

public class KafkaTopicAnnotation : IResourceAnnotation
{
	public required string TopicName { get; init; }

	public required int NumPartitions { get; init; }
}
