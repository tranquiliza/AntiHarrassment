using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AntiHarassment.Frontend.Infrastructure;
using AntiHarassment.Frontend.Application;
using AntiHarassment.SignalR.Contract;

namespace AntiHarassment.Frontend
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.ConfigureServices();
            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }

        private static void ConfigureServices(this IServiceCollection services)
        {
            services.AddSingleton<IApplicationState, ApplicationState>();
            services.AddSingleton<IApplicationStateManager, ApplicationStateManager>();
            services.AddSingleton<IChannelService, ChannelService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton(_ => new ChannelsHubSignalRClient("https://localhost:44329/"));

            services.AddSingleton<IApiGateway, ApiGateway>(x => new ApiGateway("https://localhost:44329/", x.GetRequiredService<IApplicationStateManager>(), x.GetRequiredService<HttpClient>()));
        }
    }
}
