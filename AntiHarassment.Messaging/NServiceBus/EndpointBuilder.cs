using Newtonsoft.Json;
using NServiceBus;
using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace AntiHarassment.Messaging.NServiceBus
{
    public class EndpointBuilder : IEndpointBuilder
    {
        private readonly IEndpointConfig builderConfig;
        private readonly EndpointConfiguration configuration;
        private readonly TransportExtensions<SqlServerTransport> transport;

        public EndpointConfiguration BuildConfiguration()
        {
            return configuration;
        }

        public EndpointBuilder(IEndpointConfig config)
        {
            this.builderConfig = config;

            if (string.IsNullOrEmpty(config.EndpointName))
                throw new ArgumentException("EndpointName cannot be empty.");

            if (string.IsNullOrEmpty(config.BrokerConnectionString))
                throw new ArgumentException("BrokerConnectionString cannot be empty.");

            configuration = new EndpointConfiguration(config.EndpointName);

            configuration.SendFailedMessagesTo("error");
            configuration.AuditProcessedMessagesTo("audit");

            if (Debugger.IsAttached)
                configuration.EnableInstallers();

            ConfigureConventions();
            ConfigureSerialization();
            transport = ConfigureTransport();
        }

        private void ConfigureConventions()
        {
            var conventionsBuilder = configuration.Conventions();
            conventionsBuilder.DefiningCommandsAs(t => t.Namespace.EndsWith("Commands"));
            conventionsBuilder.DefiningEventsAs(t => t.Namespace.EndsWith("Events"));
            conventionsBuilder.DefiningMessagesAs(t => t.Namespace.EndsWith("Messages"));
            conventionsBuilder.DefiningExpressMessagesAs(t => t.Namespace.EndsWith("Express"));
            conventionsBuilder.DefiningDataBusPropertiesAs(p => p.Name.StartsWith("DataBus"));
        }

        private void ConfigureSerialization()
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            var serialization = configuration.UseSerialization<NewtonsoftSerializer>();
            serialization.Settings(settings);
        }

        private TransportExtensions<SqlServerTransport> ConfigureTransport()
        {
            var transport = configuration.UseTransport<SqlServerTransport>();
            transport.ConnectionString(builderConfig.BrokerConnectionString);
            transport.Transactions(TransportTransactionMode.SendsAtomicWithReceive);
            transport.DefaultSchema("Nsb");
            return transport;
        }

        public IEndpointBuilder ConfigureRouting(Action<RoutingSettings<SqlServerTransport>> routing)
        {
            routing(transport.Routing());
            return this;
        }

        public IEndpointBuilder EnablePersistence(Action<PersistenceExtensions<SqlPersistence>> persistenceOverride = null)
        {
            if (string.IsNullOrEmpty(builderConfig.PersistenceConnectionString))
                throw new ArgumentException("PersistenceConnectionString cannot be empty.");

            var persistence = configuration.UsePersistence<SqlPersistence>();
            persistence.SqlDialect<SqlDialect.MsSqlServer>().Schema("Nsb");
            persistence.ConnectionBuilder(() => new SqlConnection(builderConfig.PersistenceConnectionString));
            persistence.SubscriptionSettings().CacheFor(TimeSpan.FromMinutes(15));

            persistenceOverride?.Invoke(persistence);

            return this;
        }
    }
}
