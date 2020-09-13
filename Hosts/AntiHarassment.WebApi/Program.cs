using AntiHarassment.Messaging.Commands;
using AntiHarassment.Messaging.NServiceBus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using Serilog;
using System;

namespace AntiHarassment.WebApi
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

            try
            {
                Log.Information("Starting Up");
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
                .UseNServiceBus(context =>
                {
                    var config = HostedEndpointConfig.ReadFrom(context.Configuration);
                    var builder = new EndpointBuilder(config);

                    builder.ConfigureRouting(routing =>
                    {
                        routing.RouteToEndpoint(typeof(LeaveChannelCommand), "AntiHarassmentChatlistener");
                        routing.RouteToEndpoint(typeof(JoinChannelCommand), "AntiHarassmentChatlistener");
                        routing.RouteToEndpoint(typeof(SendAccountConfirmationCommand), "AntiHarassmentChatlistener");
                        routing.RouteToEndpoint(typeof(SendPasswordResetTokenCommand), "AntiHarassmentChatlistener");
                        routing.RouteToEndpoint(typeof(RuleExceedCheckCommand), "AntiHarassmentChatlistener");
                    });

                    return builder.BuildConfiguration();
                })
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}
