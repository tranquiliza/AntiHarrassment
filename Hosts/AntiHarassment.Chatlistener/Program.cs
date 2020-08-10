using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AntiHarassment.Messaging.NServiceBus;
using Serilog;
using Serilog.Events;
using Serilog.Core;

namespace AntiHarassment.Chatlistener
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Starting the chat listener!");

            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application startup failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .RegisterApplicationServices()
            .ConfigureNSBEndpointWithDefaults(x => HostedEndpointConfig.ReadFrom(x.Configuration))
            .ConfigureServices(services => services.AddHostedService<ChatlistenerWorker>())
            .UseWindowsService();
    }
}
