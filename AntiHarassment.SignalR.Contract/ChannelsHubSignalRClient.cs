using AntiHarassment.SignalR.Contract.EventArgs;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
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

                hubConnection.On<string>(Methods.CHANNELJOINED, HandleChannelJoinedEvent);
                hubConnection.On<string>(Methods.CHANNELLEFT, HandleChannelLeftEvent);

                await hubConnection.StartAsync().ConfigureAwait(false);
            }
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
                await hubConnection.StopAsync();
                await hubConnection.DisposeAsync();
                hubConnection = null;
                started = false;
            }
        }
    }
}
