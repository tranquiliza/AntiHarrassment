using AntiHarassment.SignalR.Contract.EventArgs;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace AntiHarassment.SignalR.Contract
{
    public class ChannelsHubSignalRClient : IAsyncDisposable
    {
        public const string HUBURL = "/ChannelsHub";

        private readonly string hubUrl;

        private HubConnection hubConnection;

        public ChannelsHubSignalRClient(string siteUrl)
        {
            this.hubUrl = siteUrl.Trim('/') + HUBURL;
        }

        private bool started = false;

        public async Task StartAsync()
        {
            if (!started)
            {
                hubConnection = new HubConnectionBuilder()
                    .WithUrl(hubUrl)
                    .Build();

                hubConnection.On<string>(ChannelsHubMethods.CHANNELJOINED, HandleChannelJoinedEvent);
                hubConnection.On<string>(ChannelsHubMethods.CHANNELLEFT, HandleChannelLeftEvent);
                hubConnection.On<string>(ChannelsHubMethods.AUTOMODLISTENERENABLED, HandleAutoModListenerEnabledEvent);
                hubConnection.On<string>(ChannelsHubMethods.AUTOMODLISTENERDISABLED, HandleAutoModListenerDisabledEvent);

                await hubConnection.StartAsync().ConfigureAwait(false);

                started = true;
            }
        }

        public delegate void AutoModListenerDisabledEventHandler(object sender, AutoModListenerDisabledEventArgs args);
        public event EventHandler<AutoModListenerDisabledEventArgs> AutoModListenerDisabled;

        private void HandleAutoModListenerDisabledEvent(string channelName)
        {
            AutoModListenerDisabled?.Invoke(this, new AutoModListenerDisabledEventArgs { ChannelName = channelName });
        }

        public delegate void AutoModListenerEnabledEventHandler(object sender, AutoModListenerEnabledEventArgs args);
        public event EventHandler<AutoModListenerEnabledEventArgs> AutoModListenerEnabled;

        private void HandleAutoModListenerEnabledEvent(string channelName)
        {
            AutoModListenerEnabled?.Invoke(this, new AutoModListenerEnabledEventArgs { ChannelName = channelName });
        }

        public delegate void ChannelJoinedEventHandler(object sender, ChannelJoinedEventArgs args);
        public event EventHandler<ChannelJoinedEventArgs> ChannelJoined;

        private void HandleChannelJoinedEvent(string channelName)
        {
            ChannelJoined?.Invoke(this, new ChannelJoinedEventArgs { ChannelName = channelName });
        }

        public delegate void ChannelLeftEventHandler(object sender, ChannelLeftEventArgs args);
        public event EventHandler<ChannelLeftEventArgs> ChannelLeft;

        private void HandleChannelLeftEvent(string channelName)
        {
            ChannelLeft?.Invoke(this, new ChannelLeftEventArgs { ChannelName = channelName });
        }

        public async ValueTask DisposeAsync()
        {
            await StopAsync().ConfigureAwait(false);
        }

        public async Task StopAsync()
        {
            if (started)
            {
                await hubConnection.StopAsync().ConfigureAwait(false);
                await hubConnection.DisposeAsync().ConfigureAwait(false);
                hubConnection = null;
                started = false;
            }
        }
    }
}
