using AntiHarassment.SignalR.Contract.EventArgs;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
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

                //Console.WriteLine("SENDING REGISTER");
                //Console.WriteLine(twitchUsername);

                await hubConnection.SendAsync(NotificationHubMethods.REGISTER, twitchUsername).ConfigureAwait(false);

                //Console.WriteLine("SENT REGISTER");

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
                await hubConnection.StopAsync();
                await hubConnection.DisposeAsync();
                hubConnection = null;
                started = false;
            }
        }
    }
}
