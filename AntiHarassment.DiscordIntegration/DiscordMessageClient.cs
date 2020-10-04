using AntiHarassment.Core;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.DiscordIntegration
{
    public class DiscordMessageClient : IDiscordMessageClient
    {
        private readonly DiscordSocketClient client;
        private readonly DiscordRestClient restClient;
        private readonly string token;
        private readonly ulong serverId;
        private readonly ulong channelId;
        private readonly ILogger<DiscordMessageClient> logger;

        public DiscordMessageClient(IDiscordClientSettings settings, ILogger<DiscordMessageClient> logger)
        {
            this.client = new DiscordSocketClient();
            this.restClient = new DiscordRestClient();

            this.token = settings.Token;
            this.serverId = settings.ServerId;
            this.channelId = settings.ChannelId;
            this.logger = logger;

            client.Log += Client_Log;
        }

        private Task Client_Log(LogMessage arg)
        {
            logger.LogInformation("[DISCORD]: " + arg.Message);
            return Task.CompletedTask;
        }

        public async Task Initialize()
        {
            await client.LoginAsync(TokenType.Bot, token).ConfigureAwait(false);
            await restClient.LoginAsync(TokenType.Bot, token).ConfigureAwait(false);

            await client.StartAsync().ConfigureAwait(false);
        }

        public async Task SendMessageToPrometheus(string message)
        {
            var server = client.Guilds.FirstOrDefault(x => x.Id == serverId);
            if (server == null)
            {
                logger.LogWarning("Unable to send message to discord, because we cant find server");
                return;
            }

            if (!(server.Channels.FirstOrDefault(x => x.Id == channelId) is IMessageChannel channel))
            {
                logger.LogWarning("Unable to send message to discord, because we cannot find channel on the server");
                return;
            }

            await channel.SendMessageAsync(message).ConfigureAwait(false);
        }

        public async Task SendDmToUser(ulong discordUserId, string message)
        {
            var user = await restClient.GetUserAsync(discordUserId).ConfigureAwait(false);
            await user.SendMessageAsync(message).ConfigureAwait(false);
        }

        public async ValueTask DisposeAsync()
        {
            await client.StopAsync().ConfigureAwait(false);
            client.Dispose();
        }
    }
}
