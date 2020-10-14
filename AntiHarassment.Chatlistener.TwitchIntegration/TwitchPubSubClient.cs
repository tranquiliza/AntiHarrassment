using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Chatlistener.Core.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Users;

namespace AntiHarassment.Chatlistener.TwitchIntegration
{
    public class TwitchPubSubClient : IPubSubClient
    {
        private readonly TwitchAPI twitchApi;
        private readonly TwitchClientSettings twitchClientSettings;
        private readonly ILogger<TwitchPubSubClient> logger;

        private readonly List<TwitchPubSubConnection> pubSubConnections = new List<TwitchPubSubConnection>();

        private User BotUser { get; set; }

        public event EventHandler<MessageReceivedEvent> OnMessageReceived;
        public event EventHandler<MessageDeletedEvent> OnMessageDeleted;

        public event EventHandler<UserTimedoutEvent> OnUserTimedout;
        public event EventHandler<UserUntimedoutEvent> OnUserUnTimedout;

        public event EventHandler<UserBannedEvent> OnUserBanned;
        public event EventHandler<UserUnbannedEvent> OnUserUnbanned;

        public TwitchPubSubClient(TwitchClientSettings twitchClientSettings, ILogger<TwitchPubSubClient> logger)
        {
            this.twitchClientSettings = twitchClientSettings;
            twitchApi = new TwitchAPI();
            this.logger = logger;
        }

        public void Connect()
        {
            twitchApi.Helix.Settings.ClientId = twitchClientSettings.ClientId;
            twitchApi.Helix.Settings.Secret = twitchClientSettings.Secret;
        }

        public void Disconnect()
        {
            foreach (var pubSubConnection in pubSubConnections)
                pubSubConnection.Disconnect();
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
                if (!await JoinPubSubForUser(user).ConfigureAwait(false))
                    logger.LogWarning("Was unable to join pubsub for user {arg}", user.DisplayName);
            }

            return true;
        }

        public async Task<bool> JoinChannel(string channelName)
        {
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

            var userToConnect = Array.Find(response.Users, x => x != BotUser);
            if (userToConnect == null)
            {
                logger.LogWarning("Was unable to find user Id for user we're trying to connect");
                return false;
            }

            return await JoinPubSubForUser(userToConnect).ConfigureAwait(false);
        }

        private async Task<bool> JoinPubSubForUser(User userToConnect)
        {
            if (pubSubConnections.Any(x => x.IsListeningForUser(userToConnect.Id)))
            {
                logger.LogInformation("Already listening for user {arg}", userToConnect.DisplayName);
                return false;
            }

            var firstConnectionWithSpace = pubSubConnections.Find(x => x.HasSpace);
            if (firstConnectionWithSpace != null)
            {
                if (firstConnectionWithSpace.JoinChannel(BotUser.Id, userToConnect.Id, userToConnect.DisplayName))
                    return true;

                logger.LogError("Unable to connect channel to the first connection that had space, trying with new connection");
            }

            if (pubSubConnections.Count < 11)
            {
                var newConnection = new TwitchPubSubConnection(twitchClientSettings);
                await newConnection.Connect().ConfigureAwait(false);

                newConnection.OnMessageReceived += NewConnection_OnMessageReceived;
                newConnection.OnMessageDeleted += NewConnection_OnMessageDeleted;

                newConnection.OnUserBanned += NewConnection_OnUserBanned;
                newConnection.OnUserTimedout += NewConnection_OnUserTimedout;
                newConnection.OnUserUnbanned += NewConnection_OnUserUnbanned;
                newConnection.OnUserUntimedout += NewConnection_OnUserUntimedout;

                pubSubConnections.Add(newConnection);

                if (!newConnection.JoinChannel(BotUser.Id, userToConnect.Id, userToConnect.DisplayName))
                {
                    logger.LogError("Created new connection, but was unable to connect pubsub for channel: {arg}", userToConnect.DisplayName);
                    return false;
                }

                return true;
            }
            else
            {
                logger.LogWarning("Was unable to connect to channel {arg}, because we have reached maximum pubsub topics in the connections");
            }

            return false;
        }

        private void NewConnection_OnMessageDeleted(object _, MessageDeletedEvent e)
        {
            logger.LogInformation("Received deleted message from channel {arg}, on user {arg2}, issued by {arg3} regarding message: {arg4}", e.Channel, e.Username, e.DeletedBy, e.Message);
            OnMessageDeleted?.Invoke(this, e);
        }

        private void NewConnection_OnUserUntimedout(object sender, UserUntimedoutEvent e)
        {
            logger.LogInformation("Received untimeout on {arg}, issued by {arg2} in channel {arg3}", e.Username, e.UntimedoutBy, e.Channel);
            OnUserUnTimedout?.Invoke(this, e);
        }

        private void NewConnection_OnUserUnbanned(object sender, UserUnbannedEvent e)
        {
            logger.LogInformation("Received Unban on {arg}, issued by {arg2} in channel {arg3}", e.Username, e.UnbannedBy, e.Channel);
            OnUserUnbanned?.Invoke(this, e);
        }

        private void NewConnection_OnUserTimedout(object sender, UserTimedoutEvent e)
        {
            logger.LogInformation("Received Timedout on {arg}, issued by {arg2} in channel {arg3} for {arg4} seconds", e.Username, e.TimedoutBy, e.Channel, e.TimeoutDuration);
            OnUserTimedout?.Invoke(this, e);
        }

        private void NewConnection_OnUserBanned(object sender, UserBannedEvent e)
        {
            logger.LogInformation("Received Ban on {arg}, issued by {arg2} in channel {arg3}", e.Username, e.BannedBy, e.Channel);
            OnUserBanned?.Invoke(this, e);
        }

        private void NewConnection_OnMessageReceived(object _, MessageReceivedEvent e)
        {
            OnMessageReceived?.Invoke(this, e);
        }

        public bool LeaveChannel(string channelName)
        {
            var connection = pubSubConnections.Find(x => x.IsListeningForChannelName(channelName));
            if (connection == null)
            {
                logger.LogWarning("Attempting to disconnect a channel that was not connected: {arg}", channelName);
                return false;
            }

            if (connection.LeaveChannel(BotUser.Id, channelName))
                logger.LogInformation("Disconnect listener for pubsub on channel {arg}", channelName);
            else
                logger.LogWarning("Was unable to disconnect listener for pubsub on channel {arg}", channelName);

            if (connection.IsEmpty)
            {
                connection.Disconnect();

                connection.OnMessageReceived -= NewConnection_OnMessageReceived;
                connection.OnMessageDeleted -= NewConnection_OnMessageDeleted;
                connection.OnUserBanned -= NewConnection_OnUserBanned;
                connection.OnUserTimedout -= NewConnection_OnUserTimedout;
                connection.OnUserUnbanned -= NewConnection_OnUserUnbanned;
                connection.OnUserUntimedout -= NewConnection_OnUserUntimedout;

                pubSubConnections.Remove(connection);
            }

            return true;
        }

        public void Dispose()
        {
            foreach (var connection in pubSubConnections)
                connection.Disconnect();
        }
    }
}
