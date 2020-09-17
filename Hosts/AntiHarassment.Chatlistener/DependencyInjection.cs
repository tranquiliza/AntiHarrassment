using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Chatlistener.TwitchIntegration;
using AntiHarassment.Core;
using AntiHarassment.Core.Repositories;
using AntiHarassment.MachineLearning;
using AntiHarassment.Sql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AntiHarassment.Chatlistener
{
    public static class DependencyInjection
    {
        public static IHostBuilder RegisterApplicationServices(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((context, services) =>
            {
                var connectionString = context.Configuration["ConnectionStrings:AntiHarassmentDatabase"];
                var twitchUsername = context.Configuration["Twitch:Username"];
                var twitchBotOAuth = context.Configuration["Twitch:OAuthToken"];
                var clientId = context.Configuration["Twitch:ClientId"];
                var secret = context.Configuration["Twitch:Secret"];
                var fileStoragePath = context.Configuration["ApplicationSettings:FileStoragePath"];

                var chatClientSettings = new TwitchClientSettings(twitchUsername, twitchBotOAuth, clientId, secret);

                services.AddSingleton(chatClientSettings);
                services.AddSingleton<IChatClient, TwitchChatClient>();
                services.AddSingleton<IPubSubClient, TwitchPubSubClient>();
                services.AddSingleton<IRuleCheckService, RuleCheckService>();

                services.AddSingleton<IChatlistenerService, ChatlistenerService>();
                services.AddSingleton<IUserNotificationService, UserNotificationService>();
                services.AddSingleton<ISystemBanService, SystemBanService>();
                services.AddSingleton<IChannelRepository, ChannelRepository>(x => new ChannelRepository(connectionString, x.GetRequiredService<ILogger<ChannelRepository>>()));
                services.AddSingleton<ISuspensionRepository, SuspensionRepository>(x => new SuspensionRepository(connectionString, x.GetRequiredService<ILogger<SuspensionRepository>>()));
                services.AddSingleton<IChatRepository, ChatRepository>(x => new ChatRepository(connectionString, x.GetRequiredService<ILogger<ChatRepository>>()));
                services.AddSingleton<ITagRepository, TagRepository>(x => new TagRepository(connectionString, x.GetRequiredService<ILogger<TagRepository>>()));
                services.AddSingleton<IUserRepository, UserRepository>(x => new UserRepository(connectionString, x.GetRequiredService<ILogger<UserRepository>>()));
                services.AddSingleton<IChatterRepository, ChatterRepository>(x => new ChatterRepository(connectionString, x.GetRequiredService<ILogger<ChatterRepository>>()));
                services.AddSingleton<IDeletedMessagesRepository, DeletedMessagesRepository>(x => new DeletedMessagesRepository(connectionString, x.GetRequiredService<ILogger<DeletedMessagesRepository>>()));
                services.AddSingleton<IDataAnalyser, DataAnalyser>(x => new DataAnalyser(fileStoragePath, x.GetRequiredService<ITagRepository>(), x.GetRequiredService<ISuspensionRepository>(), x.GetRequiredService<IDatetimeProvider>(), x.GetRequiredService<ILogger<DataAnalyser>>()));

                services.AddSingleton<IDatetimeProvider, DatetimeProvider>();
            });
        }
    }
}
