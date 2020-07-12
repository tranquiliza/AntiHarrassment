using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AntiHarassment.Messaging.NServiceBus;

namespace AntiHarassment.Chatlistener
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .RegisterApplicationServices()
            .ConfigureNSBEndpointWithDefaults(x => HostedEndpointConfig.ReadFrom(x.Configuration))
            .ConfigureServices(services => services.AddHostedService<ChatlistenerWorker>())
            .UseWindowsService();
    }
}
