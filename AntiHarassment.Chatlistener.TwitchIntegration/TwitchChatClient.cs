using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Chatlistener.Core.Events;
using System;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;

namespace AntiHarassment.Chatlistener.TwitchIntegration
{
    public class TwitchChatClient : IChatClient
    {
        private readonly TwitchClient client;

        public event EventHandler<MessageReceivedEvent> OnMessageReceived;
        public event EventHandler<UserJoinedEvent> OnUserJoined;
        public event EventHandler<UserBannedEvent> OnUserBanned;
        public event EventHandler<UserTimedoutEvent> OnUserTimedout;

        public TwitchChatClient(TwitchClientSettings twitchClientSettings)
        {
            var credentials = new ConnectionCredentials(twitchClientSettings.TwitchUsername, twitchClientSettings.TwitchBotOAuth);
            client = new TwitchClient();
            client.Initialize(credentials);

            client.OnUserJoined += Client_OnUserJoined;
            client.OnUserTimedout += Client_OnUserTimedout;
            client.OnUserBanned += Client_OnUserBanned;
            client.OnMessageReceived += Client_OnMessageReceived;
        }

        private TaskCompletionSource<bool> connectionCompletionTask = new TaskCompletionSource<bool>();
        public async Task Connect()
        {
            client.OnConnected += TwitchClient_OnConnected;
            client.Connect();

            await connectionCompletionTask.Task.ConfigureAwait(false);
        }

        private void TwitchClient_OnConnected(object sender, OnConnectedArgs e)
        {
            client.OnConnected -= TwitchClient_OnConnected;

            connectionCompletionTask.SetResult(true);
            connectionCompletionTask = new TaskCompletionSource<bool>();
        }

        public Task Disconnect()
        {
            foreach (var joinedChannel in client.JoinedChannels)
                client.LeaveChannel(joinedChannel);

            client.Disconnect();

            return Task.CompletedTask;
        }

        public Task Reconnect()
        {
            client.Reconnect();

            return Task.CompletedTask;
        }

        private TaskCompletionSource<bool> joinChannelCompletionTask = new TaskCompletionSource<bool>();
        public async Task JoinChannel(string channelName)
        {
            if (!client.IsConnected)
            {
                // RACE CONDITION
                return;
            }

            if (client.JoinedChannels.Any(x => string.Equals(channelName, x.Channel, StringComparison.OrdinalIgnoreCase)))
                return;

            client.JoinChannel(channelName);
            client.OnJoinedChannel += TwitchClient_OnJoinedChannel;

            await joinChannelCompletionTask.Task.ConfigureAwait(false);
        }

        private void TwitchClient_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            client.OnJoinedChannel -= TwitchClient_OnJoinedChannel;

            joinChannelCompletionTask.SetResult(true);
            joinChannelCompletionTask = new TaskCompletionSource<bool>();
        }

        private TaskCompletionSource<bool> leaveChannelCompletionTask = new TaskCompletionSource<bool>();
        public async Task LeaveChannel(string channelName)
        {
            if (!client.IsConnected)
            {
                // RACE CONDITION
                return;
            }

            if (!client.JoinedChannels.Any(x => string.Equals(channelName, x.Channel, StringComparison.OrdinalIgnoreCase)))
                return;

            client.LeaveChannel(channelName);
            client.OnLeftChannel += TwitchClient_OnLeftChannel;
            await leaveChannelCompletionTask.Task.ConfigureAwait(false);
        }

        private void TwitchClient_OnLeftChannel(object sender, OnLeftChannelArgs e)
        {
            client.OnLeftChannel -= TwitchClient_OnLeftChannel;

            leaveChannelCompletionTask.SetResult(true);
            leaveChannelCompletionTask = new TaskCompletionSource<bool>();
        }

        public Task SendWhisper(string username, string message)
        {
            if (client.IsConnected)
                client.SendWhisper(username, message);

            return Task.CompletedTask;
        }

        public void BanUser(string username, string channelName, string systemReason)
        {
            //client.BanUser(channelName, username, systemReason);
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            OnMessageReceived?.Invoke(this, e.Map(autoModded: false));
        }

        private void Client_OnUserBanned(object sender, OnUserBannedArgs e)
        {
            OnUserBanned?.Invoke(this, e.Map());
        }

        private void Client_OnUserTimedout(object sender, OnUserTimedoutArgs e)
        {
            OnUserTimedout?.Invoke(this, e.Map());
        }

        private void Client_OnUserJoined(object sender, OnUserJoinedArgs e)
        {
            OnUserJoined?.Invoke(this, e.Map());
        }

        public void Dispose()
        {
            client.Disconnect();
        }
    }
}
