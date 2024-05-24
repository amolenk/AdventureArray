using Aspire.Hosting.Lifecycle;

namespace AdventureArray.Infrastructure.AppHost.Extensions.Kafka;

public static class KafkaBuilderExtensions
{
    /// <summary>
    /// Adds a pgAdmin 4 administration and development platform for PostgreSQL to the application model. This version the package defaults to the 8.3 tag of the dpage/pgadmin4 container image
    /// </summary>
    /// <param name="builder">The PostgreSQL server resource builder.</param>
    /// <param name="configureContainer">Callback to configure PgAdmin container resource.</param>
    /// <param name="containerName">The name of the container (Optional).</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<T> AddTopic<T>(this IResourceBuilder<T> builder, string topicName, int partitionCount = 1) where T : KafkaServerResource
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

