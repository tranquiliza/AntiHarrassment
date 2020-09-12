using AntiHarassment.Chatlistener.Core.Events;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;
using TwitchLib.PubSub.Models.Responses;
using TwitchLib.PubSub.Models.Responses.Messages;

namespace AntiHarassment.Chatlistener.TwitchIntegration
{
    public class TwitchPubSubConnection
    {
        private readonly TwitchPubSub pubSubService;
        private readonly Dictionary<string, string> UserIdChannelName = new Dictionary<string, string>();
        private readonly TwitchClientSettings twitchClientSettings;

        public bool Connected;

        public event EventHandler<MessageReceivedEvent> OnMessageReceived;
        public event EventHandler<UserBannedEvent> OnUserBanned;
        public event EventHandler<UserTimedoutEvent> OnUserTimedout;
        public event EventHandler<UserUntimedoutEvent> OnUserUntimedout;
        public event EventHandler<UserUnbannedEvent> OnUserUnbanned;

        public bool HasSpace => UserIdChannelName.Count < 50;

        public bool IsEmpty => UserIdChannelName.Count == 0;

        public TwitchPubSubConnection(TwitchClientSettings twitchClientSettings)
        {
            pubSubService = new TwitchPubSub();
            this.twitchClientSettings = twitchClientSettings;

            pubSubService.OnLog += PubSubService_OnLog;

            pubSubService.OnTimeout += PubSubService_OnTimeout;
            pubSubService.OnBan += PubSubService_OnBan;
            pubSubService.OnUntimeout += PubSubService_OnUntimeout;
            pubSubService.OnUnban += PubSubService_OnUnban;
        }

        public Task Connect()
        {
            pubSubService.Connect();
            Connected = true;

            return Task.CompletedTask;
        }

        public void Disconnect()
        {
            pubSubService.Disconnect();
            Connected = false;
        }

        public bool IsListeningForUser(string userId)
            => UserIdChannelName.ContainsKey(userId);

        public bool IsListeningForChannelName(string channelName)
        {
            var channelValuePair = UserIdChannelName.FirstOrDefault(x => string.Equals(x.Value, channelName, StringComparison.OrdinalIgnoreCase));

            return channelValuePair.Key != null;
        }

        public bool JoinChannel(string botUserId, string userId, string userDisplayName)
        {
            if (UserIdChannelName.ContainsKey(userId))
                return false;

            UserIdChannelName.Add(userId, userDisplayName);
            pubSubService.ListenToChatModeratorActions(botUserId, userId);

            pubSubService.SendTopics(twitchClientSettings.TwitchBotOAuth);

            return true;
        }

        public bool LeaveChannel(string botUserId, string channelName)
        {
            var channelValuePair = UserIdChannelName.FirstOrDefault(x => string.Equals(x.Value, channelName, StringComparison.OrdinalIgnoreCase));
            if (channelValuePair.Key == null)
                return true;

            pubSubService.ListenToChatModeratorActions(botUserId, channelValuePair.Key);
            pubSubService.SendTopics(twitchClientSettings.TwitchBotOAuth, unlisten: true);

            UserIdChannelName.Remove(channelValuePair.Key);

            return true;
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
                        return;
                    }

                    switch (cma?.ModerationAction.ToLower())
                    {
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
        }

        private void PubSubService_OnUntimeout(object sender, OnUntimeoutArgs e)
        {
            if (!UserIdChannelName.TryGetValue(e.ChannelId, out var channelName))
                return;

            OnUserUntimedout?.Invoke(this, new UserUntimedoutEvent
            {
                Username = e.UntimeoutedUser,
                UntimedoutBy = e.UntimeoutedBy,
                Channel = channelName
            });
        }

        private void PubSubService_OnUnban(object sender, OnUnbanArgs e)
        {
            if (!UserIdChannelName.TryGetValue(e.ChannelId, out var channelName))
                return;

            OnUserUnbanned?.Invoke(this, new UserUnbannedEvent
            {
                Username = e.UnbannedUser,
                UnbannedBy = e.UnbannedBy,
                Channel = channelName
            });
        }

        private void PubSubService_OnBan(object sender, OnBanArgs e)
        {
            if (!UserIdChannelName.TryGetValue(e.ChannelId, out var channelName))
                return;

            OnUserBanned?.Invoke(this, new UserBannedEvent
            {
                BanReason = e.BanReason,
                Channel = channelName,
                Username = e.BannedUser,
                BannedBy = e.BannedBy
            });
        }

        private void PubSubService_OnTimeout(object sender, OnTimeoutArgs e)
        {
            if (!UserIdChannelName.TryGetValue(e.ChannelId, out var channelName))
                return;

            OnUserTimedout?.Invoke(this, new UserTimedoutEvent
            {
                Username = e.TimedoutUser,
                Channel = channelName,
                TimeoutDuration = (int)e.TimeoutDuration.TotalSeconds,
                TimeoutReason = e.TimeoutReason,
                TimedoutBy = e.TimedoutBy
            });
        }
    }
}
