using AntiHarassment.Core;
using AntiHarassment.FileSystem;
using AntiHarassment.Messaging.NServiceBus;
using AntiHarassment.Sql;
using AntiHarassment.TwitchIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AntiHarassment.WebApi
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services, IConfiguration configuration, ApplicationConfiguration applicationConfiguration)
        {
            var connstring = configuration["ConnectionStrings:AntiHarassmentDatabase"];

            var applicationSecret = configuration["Twitch:Secret"];
            var clientId = configuration["Twitch:ClientId"];
            var redirectionUri = configuration["ApplicationSettings:RedirectUri"];

            var twitchApiConfiguration = new TwitchApiSettings { ClientId = clientId, Secret = applicationSecret, RedirectionUrl = redirectionUri };
            services.AddSingleton(twitchApiConfiguration);
            services.AddSingleton(new HttpClient());
            services.AddSingleton<IDatetimeProvider, DatetimeProvider>();

            var fileStoragePath = configuration["ApplicationSettings:FileStoragePath"];
            services.AddSingleton<IFileRepository, FileRepository>(_ => new FileRepository(fileStoragePath));

            services.AddSingleton<IChannelService, ChannelService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<ISuspensionService, SuspensionService>();
            services.AddSingleton<ITagService, TagService>();
            services.AddSingleton<IUserReportService, UserReportService>();
            services.AddSingleton<INotificationService, NotificationService>();

            services.AddSingleton<ISecurity, PasswordSecurity>();
            services.AddSingleton<IApplicationConfiguration>(applicationConfiguration);
            services.AddSingleton<IChannelReportService, ChannelReportService>();
            services.AddSingleton<ISystemReportService, SystemReportService>();

            services.AddSingleton<IChannelRepository, ChannelRepository>(x => new ChannelRepository(connstring, x.GetRequiredService<ILogger<ChannelRepository>>()));
            services.AddSingleton<IUserRepository, UserRepository>(x => new UserRepository(connstring, x.GetRequiredService<ILogger<UserRepository>>()));
            services.AddSingleton<ISuspensionRepository, SuspensionRepository>(x => new SuspensionRepository(connstring, x.GetRequiredService<ILogger<SuspensionRepository>>()));
            services.AddSingleton<ITagRepository, TagRepository>(x => new TagRepository(connstring, x.GetRequiredService<ILogger<TagRepository>>()));
            services.AddSingleton<IChatRepository, ChatRepository>(x => new ChatRepository(connstring, x.GetRequiredService<ILogger<ChatRepository>>()));

            services.AddSingleton<IMessageDispatcher, MessageDispatcher>();

            services.AddSingleton<ITwitchApiIntegration, TwitchApiWrapper>();

            return services;
        }
    }
}
