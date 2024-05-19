using Confluent.Kafka;

namespace AdventureArray.Infrastructure.Messaging.Configuration;

/// <summary>
/// Provides the client configuration settings for configuring Kafka.
/// </summary>
public sealed class KafkaSettings
{
	public const string SectionName = "Kafka";

	public string? Host { get; init; }

	public SaslMechanism? SaslMechanism { get; init; }

	public SecurityProtocol? SecurityProtocol { get; init; }

	public string? SaslUsername { get; init; }

	public string? SaslPassword { get; init; }

	public string? ConsumerGroupId { get; init; }
}
