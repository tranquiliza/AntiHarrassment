using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Chatlistener.Core.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Users;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;
using TwitchLib.PubSub.Models.Responses;
using TwitchLib.PubSub.Models.Responses.Messages;

namespace AntiHarassment.Chatlistener.TwitchIntegration
{
    public class TwitchPubSubClient : IPubSubClient
    {
        // TODO Need to make sure that pubsub server never exceeds 50 topics. 
        // When time arrives, refactor to have a collection of pubsubservice, wrapped with information about active topics
        // maximum of 10 per IP.
        private readonly TwitchPubSub pubSubService;
        private readonly TwitchAPI twitchApi;
        private readonly TwitchClientSettings twitchClientSettings;
        private readonly ILogger<TwitchPubSubClient> logger;

        private User BotUser { get; set; }

        public event EventHandler<MessageReceivedEvent> OnMessageReceived;
        //public event EventHandler<UserJoinedEvent> OnUserJoined;
        //public event EventHandler<UserBannedEvent> OnUserBanned;
        //public event EventHandler<UserTimedoutEvent> OnUserTimedout;

        private Dictionary<string, string> UserIdChannelName = new Dictionary<string, string>();

        public TwitchPubSubClient(TwitchClientSettings twitchClientSettings, ILogger<TwitchPubSubClient> logger)
        {
            this.twitchClientSettings = twitchClientSettings;
            pubSubService = new TwitchPubSub();
            twitchApi = new TwitchAPI();

            //pubSubService.OnTimeout += PubSubService_OnTimeout;
            //pubSubService.OnBan += PubSubService_OnBan;
            //pubSubService.OnUntimeout += PubSubService_OnUntimeout;
            //pubSubService.OnUnban += PubSubService_OnUnban;
            pubSubService.OnLog += PubSubService_OnLog;
            this.logger = logger;
        }

        public Task Connect()
        {
            twitchApi.Helix.Settings.ClientId = twitchClientSettings.ClientId;
            twitchApi.Helix.Settings.Secret = twitchClientSettings.Secret;
            pubSubService.Connect();
            return Task.CompletedTask;
        }

        public async Task<bool> JoinChannels(List<string> channelNames)
        {
            channelNames.Add(twitchClientSettings.TwitchUsername);
            // TODO THIS IS FRAGILE, NEEDS TO MAKE SURE ONLY 100 AT MAX PER REQUEST!
            var response = await twitchApi.Helix.Users.GetUsersAsync(logins: channelNames).ConfigureAwait(false);

            if (BotUser == null)
                BotUser = Array.Find(response.Users, x => string.Equals(x.DisplayName, twitchClientSettings.TwitchUsername, StringComparison.OrdinalIgnoreCase));

            if (BotUser == null)
            {
                logger.LogWarning("Was unable to find bot users Id");
                return false;
            }

            foreach (var user in response.Users.Where(x => x != BotUser))
            {
                if (UserIdChannelName.Count == 50)
                {
                    logger.LogWarning("Channel count at 50, cannot listen to more topics on this connection");
                    continue;
                }

                if (UserIdChannelName.ContainsKey(user.Id))
                {
                    logger.LogInformation("Channel already in pubsub listening, skipping: {arg}", user.DisplayName);
                    continue;
                }

                UserIdChannelName.Add(user.Id, user.DisplayName);
                pubSubService.ListenToChatModeratorActions(BotUser.Id, user.Id);
            }

            pubSubService.SendTopics(twitchClientSettings.TwitchBotOAuth);

            return true;
        }

        public async Task<bool> JoinChannel(string channelName)
        {
            if (UserIdChannelName.Count == 50)
            {
                logger.LogWarning("Channel count at 50, cannot listen to more topics on this connection");
                return false;
            }

            var channelNames = new List<string> { channelName };
            if (BotUser == null)
                channelNames.Add(twitchClientSettings.TwitchUsername);

            var response = await twitchApi.Helix.Users.GetUsersAsync(logins: channelNames).ConfigureAwait(false);

            if (BotUser == null)
                BotUser = Array.Find(response.Users, x => string.Equals(x.DisplayName == twitchClientSettings.TwitchUsername, StringComparison.OrdinalIgnoreCase));

            if (BotUser == null)
            {
                logger.LogWarning("Was unable to find bot users Id");
                return false;
            }

            foreach (var user in response.Users.Where(x => x != BotUser))
            {
                if (UserIdChannelName.ContainsKey(user.Id))
                {
                    logger.LogInformation("Channel already in pubsub listening, skipping: {arg}", user.DisplayName);
                    return false;
                }

                UserIdChannelName.Add(user.Id, user.DisplayName);
                pubSubService.ListenToChatModeratorActions(BotUser.Id, user.Id);
            }

            pubSubService.SendTopics(twitchClientSettings.TwitchBotOAuth);
            return true;
        }

        public bool LeaveChannel(string channelName)
        {
            var channelValuePair = UserIdChannelName.FirstOrDefault(x => string.Equals(x.Value, channelName, StringComparison.OrdinalIgnoreCase));
            if (channelValuePair.Key == null)
                return false;

            pubSubService.ListenToChatModeratorActions(BotUser.Id, channelValuePair.Key);
            pubSubService.SendTopics(twitchClientSettings.TwitchBotOAuth, unlisten: true);

            UserIdChannelName.Remove(channelValuePair.Key);

            return true;
        }

        //private void PubSubService_OnUntimeout(object sender, OnUntimeoutArgs e)
        //{
        //}

        //private void PubSubService_OnUnban(object sender, OnUnbanArgs e)
        //{
        //}

        //private void PubSubService_OnBan(object sender, OnBanArgs e)
        //{
        //}

        //private void PubSubService_OnTimeout(object sender, OnTimeoutArgs e)
        //{
        //}

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
            logger.LogDebug("Received a message that is unaccounted for: {arg}", message);
        }

        public void Dispose()
        {
            pubSubService.Disconnect();
        }
    }
}
