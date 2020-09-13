using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using System;

namespace AntiHarassment.Messaging.NServiceBus
{
    public static class EndpointBuilderExtensions
    {
        public static IHostBuilder ConfigureNSBEndpointWithDefaults(this IHostBuilder hostBuilder,
               Func<HostBuilderContext, IEndpointConfig> configFactory,
               Action<IEndpointBuilder> endpointBuilder = null)
        {
            hostBuilder.UseNServiceBus(hostingContext =>
            {
                var config = configFactory(hostingContext);

                var builder = new EndpointBuilder(config);
                endpointBuilder?.Invoke(builder);
                return builder.BuildConfiguration();
            });

            return hostBuilder.ConfigureServices(services => services.AddSingleton<IMessageDispatcher, MessageDispatcher>());
        }
    }
}
