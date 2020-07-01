using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiHarassment.Messaging.Commands;
using AntiHarassment.Messaging.NServiceBus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace AntiHarassment.WebApi
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
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
