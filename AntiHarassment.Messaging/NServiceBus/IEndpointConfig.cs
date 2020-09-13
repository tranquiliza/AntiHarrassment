namespace AntiHarassment.Messaging.NServiceBus
{
    public interface IEndpointConfig
    {
        /// <summary>
        /// Connection string to the SQL Server database used for transport of messages, handling subscriptions etc.
        /// </summary>
        string BrokerConnectionString { get; }

        /// <summary>
        /// Endpoint's primary identifier. This name is also used for the queue, that messages are transported to and from.
        /// </summary>
        string EndpointName { get; }

        /// <summary>
        /// If the endpoint uses persistence (enabled trough the builder), a seperate connection string is needed to reference the SQL Server database storing the persistence data.
        /// </summary>
        string PersistenceConnectionString { get; }
    }
}
