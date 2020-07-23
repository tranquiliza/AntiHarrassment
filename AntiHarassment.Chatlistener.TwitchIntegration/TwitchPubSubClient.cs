using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Core.Interfaces;
using TwitchLib.PubSub;

namespace AntiHarassment.Chatlistener.TwitchIntegration
{
    public class TwitchPubSubClient : IPubSubClient
    {
        private readonly TwitchPubSub pubSubService;
        private readonly TwitchAPI twitchApi;
        private readonly TwitchClientSettings twitchClientSettings;

        public TwitchPubSubClient(TwitchClientSettings twitchClientSettings)
        {
            this.twitchClientSettings = twitchClientSettings;
            pubSubService = new TwitchPubSub();
            twitchApi = new TwitchAPI();

            pubSubService.OnTimeout += PubSubService_OnTimeout;
            pubSubService.OnBan += PubSubService_OnBan;
            pubSubService.OnLog += PubSubService_OnLog;
            pubSubService.OnUntimeout += PubSubService_OnUntimeout;
            pubSubService.OnUnban += PubSubService_OnUnban;
        }

        private void PubSubService_OnUntimeout(object sender, TwitchLib.PubSub.Events.OnUntimeoutArgs e)
        {
            Console.WriteLine("UNTIMEOUT: " + e.UntimeoutedUser);
        }

        private void PubSubService_OnUnban(object sender, TwitchLib.PubSub.Events.OnUnbanArgs e)
        {
            Console.WriteLine("UNBAN: " + e.UnbannedUser);
        }

        private void PubSubService_OnLog(object sender, TwitchLib.PubSub.Events.OnLogArgs e)
        {
            Console.WriteLine(e.Data);
        }

        private void PubSubService_OnBan(object sender, TwitchLib.PubSub.Events.OnBanArgs e)
        {
            Console.WriteLine("BAN: " + e.BannedUser);
        }

        private void PubSubService_OnTimeout(object sender, TwitchLib.PubSub.Events.OnTimeoutArgs e)
        {
            Console.WriteLine("TIMEOUT: " + e.TimedoutUser);
        }

        public Task Connect()
        {
            twitchApi.Helix.Settings.ClientId = twitchClientSettings.ClientId;
            twitchApi.Helix.Settings.Secret = twitchClientSettings.Secret;
            pubSubService.Connect();
            return Task.CompletedTask;
        }

        public async Task JoinChannels(List<string> channelNames)
        {
            channelNames.Add(twitchClientSettings.TwitchUsername);
            var response = await twitchApi.Helix.Users.GetUsersAsync(logins: channelNames).ConfigureAwait(false);

            var botUser = response.Users.FirstOrDefault(x => string.Equals(x.DisplayName, twitchClientSettings.TwitchUsername, StringComparison.OrdinalIgnoreCase));
            if (botUser == null)
            {
                // SHIT
                return;
            }

            foreach (var user in response.Users)
            {
                pubSubService.ListenToChatModeratorActions(botUser.Id, user.Id);
            }

            pubSubService.SendTopics(twitchClientSettings.TwitchBotOAuth);
        }

        public void Dispose()
        {
            pubSubService.Disconnect();
        }
    }
}
