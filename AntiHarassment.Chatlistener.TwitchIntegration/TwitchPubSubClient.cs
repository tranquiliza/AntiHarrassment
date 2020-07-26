using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Chatlistener.Core.Events;
using AntiHarassment.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;
using TwitchLib.PubSub.Models.Responses;
using TwitchLib.PubSub.Models.Responses.Messages;

namespace AntiHarassment.Chatlistener.TwitchIntegration
{
    public class TwitchPubSubClient : IPubSubClient
    {
        private readonly TwitchPubSub pubSubService;
        private readonly TwitchAPI twitchApi;
        private readonly TwitchClientSettings twitchClientSettings;

        public event EventHandler<MessageReceivedEvent> OnMessageReceived;
        //public event EventHandler<UserJoinedEvent> OnUserJoined;
        //public event EventHandler<UserBannedEvent> OnUserBanned;
        //public event EventHandler<UserTimedoutEvent> OnUserTimedout;

        private Dictionary<string, string> UserIdChannelName = new Dictionary<string, string>();

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

            var botUser = Array.Find(response.Users, x => string.Equals(x.DisplayName, twitchClientSettings.TwitchUsername, StringComparison.OrdinalIgnoreCase));
            if (botUser == null)
            {
                // SHIT
                return;
            }

            foreach (var user in response.Users)
            {
                UserIdChannelName.Add(user.Id, user.DisplayName);
                pubSubService.ListenToChatModeratorActions(botUser.Id, user.Id);
            }

            pubSubService.SendTopics(twitchClientSettings.TwitchBotOAuth);
        }

        private void PubSubService_OnUntimeout(object sender, OnUntimeoutArgs e)
        {
            Console.WriteLine("UNTIMEOUT: " + e.UntimeoutedUser);
        }

        private void PubSubService_OnUnban(object sender, OnUnbanArgs e)
        {
            Console.WriteLine("UNBAN: " + e.UnbannedUser);
        }

        private void PubSubService_OnBan(object sender, OnBanArgs e)
        {
            Console.WriteLine("BAN: " + e.BannedUser);
        }

        private void PubSubService_OnTimeout(object sender, OnTimeoutArgs e)
        {
            Console.WriteLine("TIMEOUT: " + e.TimedoutUser);
        }

        private void PubSubService_OnLog(object sender, OnLogArgs e)
        {
            var message = e.Data;
            var type = JObject.Parse(message).SelectToken("type")?.ToString();
            if (type?.ToLower() != "message")
                return;

            var msg = new Message(message);
            switch (msg.Topic.Split('.')[0])
            {
                case "chat_moderator_actions":
                    var cma = msg.MessageData as ChatModeratorActions;
                    var reason = "";
                    var targetChannelId = msg.Topic.Split('.')[2];
                    if (!UserIdChannelName.TryGetValue(targetChannelId, out var targetChannelName))
                    {
                        Console.WriteLine("Unable to match targetChannelId, to a channel?: " + targetChannelId);
                        return;
                    }

                    switch (cma?.ModerationAction.ToLower())
                    {
                        case "timeout":
                            //if (cma.Args.Count > 2)
                            //    reason = cma.Args[2];
                            //OnTimeout?.Invoke(this, new OnTimeoutArgs
                            //{
                            //    TimedoutBy = cma.CreatedBy,
                            //    TimedoutById = cma.CreatedByUserId,
                            //    TimedoutUserId = cma.TargetUserId,
                            //    TimeoutDuration = TimeSpan.FromSeconds(int.Parse(cma.Args[1])),
                            //    TimeoutReason = reason,
                            //    TimedoutUser = cma.Args[0],
                            //    ChannelId = channelId
                            //});
                            return;
                        case "ban":
                            //if (cma.Args.Count > 1)
                            //    reason = cma.Args[1];
                            //OnBan?.Invoke(this, new OnBanArgs { BannedBy = cma.CreatedBy, BannedByUserId = cma.CreatedByUserId, BannedUserId = cma.TargetUserId, BanReason = reason, BannedUser = cma.Args[0], ChannelId = channelId });
                            return;
                        //case "delete":
                        //    OnMessageDeleted?.Invoke(this, new OnMessageDeletedArgs { DeletedBy = cma.CreatedBy, DeletedByUserId = cma.CreatedByUserId, TargetUserId = cma.TargetUserId, TargetUser = cma.Args[0], Message = cma.Args[1], MessageId = cma.Args[2], ChannelId = channelId });
                        //    return;
                        case "unban":
                            //OnUnban?.Invoke(this, new OnUnbanArgs { UnbannedBy = cma.CreatedBy, UnbannedByUserId = cma.CreatedByUserId, UnbannedUserId = cma.TargetUserId, UnbannedUser = cma.Args[0], ChannelId = channelId });
                            return;
                        case "untimeout":
                            //OnUntimeout?.Invoke(this, new OnUntimeoutArgs { UntimeoutedBy = cma.CreatedBy, UntimeoutedByUserId = cma.CreatedByUserId, UntimeoutedUserId = cma.TargetUserId, UntimeoutedUser = cma.Args[0], ChannelId = channelId });
                            return;
                        case "automod_rejected":
                            OnMessageReceived?.Invoke(this, new MessageReceivedEvent
                            {
                                Message = cma.Args[1],
                                DisplayName = cma.Args[0],
                                UserId = cma.TargetUserId,
                                Channel = targetChannelName,
                                AutoModded = true
                            });
                            return;
                    }
                    break;
            }

            UnaccountedFor(message);
        }

        private void UnaccountedFor(string message)
        {
            Console.WriteLine(message);
        }

        public void Dispose()
        {
            pubSubService.Disconnect();
        }
    }
}
