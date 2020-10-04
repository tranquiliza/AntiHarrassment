using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Chatlistener.TwitchIntegration;
using AntiHarassment.Core;
using AntiHarassment.Core.Repositories;
using AntiHarassment.DiscordIntegration;
using AntiHarassment.MachineLearning;
using AntiHarassment.Sql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AntiHarassment.Chatlistener
{

    internal class DiscordClientSettings : IDiscordClientSettings
    {
        public string Token { get; }

        public ulong ServerId { get; }

        public ulong ChannelId { get; }

        public DiscordClientSettings(string token, ulong serverId, ulong channelId)
        {
            Token = token;
            ServerId = serverId;
            ChannelId = channelId;
        }
    }
}
