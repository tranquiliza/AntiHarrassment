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
using System.IdentityModel.Tokens.Jwt;

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
            // Line because optimazation is eating everything?
            _ = new JwtHeader();
            _ = new JwtPayload();

#if DEBUG
            const string apiUrl = "https://localhost:44329/";
#else
            const string apiUrl = "https://tranquiliza.dynu.net/AntiHarassmentApi/";
#endif
            services.AddSingleton<IApplicationState, ApplicationState>();
            services.AddSingleton<IApplicationStateManager, ApplicationStateManager>();
            services.AddSingleton<IAdminChannelService, AdminChannelService>();
            services.AddSingleton<IUserChannelService, UserChannelService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<ISuspensionService, SuspensionService>();
            services.AddSingleton<ITagService, TagService>();
            services.AddSingleton<IUserReportService, UserReportService>();

            services.AddSingleton(_ => new ChannelsHubSignalRClient(apiUrl));
            services.AddSingleton(_ => new SuspensionsHubSignalRClient(apiUrl));
            services.AddSingleton<IApiGateway, ApiGateway>(x => new ApiGateway(apiUrl, x.GetRequiredService<IApplicationStateManager>(), x.GetRequiredService<HttpClient>()));
        }
    }
}
