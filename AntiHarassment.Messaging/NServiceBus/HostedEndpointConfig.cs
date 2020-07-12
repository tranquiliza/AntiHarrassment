using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Messaging.NServiceBus
{
    public sealed class HostedEndpointConfig : IEndpointConfig
    {
        public string EndpointName { get; private set; }
        public string BrokerConnectionString { get; private set; }
        public string PersistenceConnectionString { get; private set; }

        private HostedEndpointConfig() { }

        public static IEndpointConfig ReadFrom(IConfiguration configuration)
            => new HostedEndpointConfig
            {
                EndpointName = configuration["NSB:EndpointName"],
                BrokerConnectionString = configuration["NSB:BrokerConnectionString"],
                PersistenceConnectionString = configuration["NSB:PersistenceConnectionString"]
            };
    }
}
