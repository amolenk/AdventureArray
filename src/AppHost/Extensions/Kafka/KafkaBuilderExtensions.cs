using Aspire.Hosting.Lifecycle;

namespace AdventureArray.Infrastructure.AppHost.Extensions.Kafka;

public static class KafkaBuilderExtensions
{
    public static IResourceBuilder<T> AddTopic<T>(
	    this IResourceBuilder<T> builder,
	    string topicName, int partitionCount = 1)
	    where T : KafkaServerResource
    {
	    builder.WithAnnotation(new KafkaTopicAnnotation
	    {
		    TopicName = topicName,
		    NumPartitions = partitionCount
	    });

	    builder.ApplicationBuilder.Services.TryAddLifecycleHook<KafkaTopicCreatorHook>();

        return builder;
    }
}

