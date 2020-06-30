using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Chatlistener.TwitchIntegration;
using AntiHarassment.Core;
using AntiHarassment.Sql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Chatlistener
{
    public static class DependencyInjection
    {
        public static IHostBuilder RegisterApplicationServices(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((context, services) =>
            {
                var connectionString = context.Configuration["ConnectionStrings:ChatListenerDatabase"];
                var twitchUsername = context.Configuration["Twitch:Username"];
                var twitchBotOAuth = context.Configuration["Twitch:OAuthToken"];

                var chatClientSettings = new TwitchClientSettings(twitchUsername, twitchBotOAuth);

                services.AddSingleton(chatClientSettings);
                services.AddSingleton<IChatClient, TwitchChatClient>();

                services.AddSingleton<IChatlistenerService, ChatlistenerService>();
                services.AddSingleton<IChannelRepository, ChannelRepository>(_ => new ChannelRepository(connectionString));
                services.AddSingleton<ISuspensionRepository, SuspensionRepository>(_ => new SuspensionRepository(connectionString));
                services.AddSingleton<IChatRepository, ChatRepository>(_ => new ChatRepository(connectionString));

                services.AddSingleton<IDatetimeProvider, DatetimeProvider>();
            });
        }
    }
}
