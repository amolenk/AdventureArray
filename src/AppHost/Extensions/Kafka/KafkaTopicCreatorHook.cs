using System.Diagnostics.CodeAnalysis;
using Aspire.Hosting.Lifecycle;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Logging;

namespace AdventureArray.Infrastructure.AppHost.Extensions.Kafka;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
internal sealed class KafkaTopicCreatorHook : IDistributedApplicationLifecycleHook
{
	private static readonly TimeSpan RetryDelay = TimeSpan.FromMilliseconds(250);

	private readonly ILogger<KafkaTopicCreatorHook> _logger;

	public KafkaTopicCreatorHook(ILogger<KafkaTopicCreatorHook> logger)
	{
		ArgumentNullException.ThrowIfNull(logger);

		_logger = logger;
	}

	public async Task AfterResourcesCreatedAsync(DistributedApplicationModel appModel,
		CancellationToken cancellationToken = default)
	{
		var kafkaResources = appModel.Resources.OfType<KafkaServerResource>();
		foreach (var kafkaResource in kafkaResources)
		{
			if (!kafkaResource.TryGetAnnotationsOfType<KafkaTopicAnnotation>(
				    out var kafkaTopicAnnotations)) continue;

			var topicSpecifications = kafkaTopicAnnotations
				.Select(a => new TopicSpecification
				{
					Name = a.TopicName,
					NumPartitions = a.NumPartitions
				});

			var connectionString = await kafkaResource.ConnectionStringExpression.GetValueAsync(cancellationToken);
			if (connectionString is null) continue;

			await TryCreateTopicsUntilSuccessfulAsync(connectionString, topicSpecifications, cancellationToken);
		}
    }

	private async Task TryCreateTopicsUntilSuccessfulAsync(string bootstrapServers,
		IEnumerable<TopicSpecification> topicSpecifications, CancellationToken cancellationToken)
	{
		var topicSpecificationList = topicSpecifications.ToList();

		while (!cancellationToken.IsCancellationRequested)
		{
			try
			{
				await CreateTopicsThatDoNotExistAsync(bootstrapServers, topicSpecificationList);
				return;
			}
			catch (KafkaException e) when (e.Error.IsLocalError)
			{
				_logger.LogWarning("Failed to create topics, retrying in {RetryDelayMs}ms",
					RetryDelay.TotalMilliseconds);
			}

			await Task.Delay(RetryDelay, cancellationToken);
		}
	}

	private async Task CreateTopicsThatDoNotExistAsync(string bootstrapServers,
		IEnumerable<TopicSpecification> topicSpecifications)
	{
		using var adminClient = new AdminClientBuilder(new AdminClientConfig
		{
			BootstrapServers = bootstrapServers
		}).Build();

		var existingTopicNames = adminClient.GetMetadata(TimeSpan.FromSeconds(5))
			.Topics.Select(t => t.Topic).ToList();

		var newTopics = topicSpecifications.Where(t => !existingTopicNames.Contains(t.Name)).ToList();

		await adminClient.CreateTopicsAsync(newTopics);

		_logger.LogInformation("Created topic(s): {TopicNames}",
			newTopics.Select(t => t.Name));
	}
}
