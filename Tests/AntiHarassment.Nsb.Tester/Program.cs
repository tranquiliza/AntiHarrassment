using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiHarassment.Messaging.Commands;
using AntiHarassment.Messaging.NServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;

namespace AntiHarassment.Nsb.Tester
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseNServiceBus(context =>
                {
                    var config = HostedEndpointConfig.ReadFrom(context.Configuration);
                    var builder = new EndpointBuilder(config);

                    builder.ConfigureRouting(routing =>
                    {
                        routing.RouteToEndpoint(typeof(LeaveChannelCommand), "AntiHarassmentChatlistener");
                        routing.RouteToEndpoint(typeof(JoinChannelCommand), "AntiHarassmentChatlistener");
                    });


                    return builder.BuildConfiguration();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IMessageDispatcher, MessageDispatcher>();
                    services.AddHostedService<Worker>();
                });
    }
}
