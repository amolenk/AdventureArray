namespace AdventureArray.Infrastructure.Messaging.Extensions;

public static class RiderRegistrationConfiguratorExtensions
{
	public static void AddProducerIfNotExists<TKey, TValue>(this IRiderRegistrationConfigurator configurator,
		string topicName,
		Action<IRiderRegistrationContext, IKafkaProducerConfigurator<TKey, TValue>>? configure = null)
		where TValue : class
	{
		if (RegisteredProducerTopics.IsTopicRegistered(topicName)) return;

		configurator.AddProducer(topicName, configure);

		RegisteredProducerTopics.AddTopic(topicName);
	}

	private static class RegisteredProducerTopics
	{
		private static readonly List<string> TopicNames = [];

		public static void AddTopic(string topicName)
		{
			TopicNames.Add(topicName);
		}

		public static bool IsTopicRegistered(string topicName)
		{
			return TopicNames.Contains(topicName);
		}
	}
}

