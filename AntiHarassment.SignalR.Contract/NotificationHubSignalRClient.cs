using AntiHarassment.SignalR.Contract.EventArgs;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace AntiHarassment.SignalR.Contract
{
    public class NotificationHubSignalRClient : IAsyncDisposable
    {
        public const string HUBURL = "/NotificationHub";

        private readonly string hubUrl;

        private HubConnection hubConnection;

        public NotificationHubSignalRClient(string siteUrl)
        {
            this.hubUrl = siteUrl.Trim('/') + HUBURL;
        }

        private bool started = false;

        public async Task StartAsync(string twitchUsername)
        {
            if (!started)
            {
                hubConnection = new HubConnectionBuilder()
                    .WithUrl(hubUrl)
                    .Build();

                hubConnection.On<string, string, string>(NotificationHubMethods.NOTIFY, HandleNotification);

                await hubConnection.StartAsync().ConfigureAwait(false);

                await hubConnection.SendAsync(NotificationHubMethods.REGISTER, twitchUsername).ConfigureAwait(false);

                started = true;
            }
        }

        public delegate void NotificationEventReceivedEventHandler(object sender, NotificationEventArgs args);
        public event EventHandler<NotificationEventArgs> NotificationReceived;

        private void HandleNotification(string username, string ruleName, string channelOfOrigin)
        {
            NotificationReceived?.Invoke(this, new NotificationEventArgs { Username = username, RuleName = ruleName, ChannelOfOrigin = channelOfOrigin });
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
