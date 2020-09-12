using AntiHarassment.SignalR.Contract.EventArgs;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace AntiHarassment.SignalR.Contract
{
    public class SuspensionsHubSignalRClient : IAsyncDisposable
    {
        public const string HUBURL = "/SuspensionsHub";

        private readonly string hubUrl;

        private HubConnection hubConnection;

        public SuspensionsHubSignalRClient(string siteUrl)
        {
            hubUrl = siteUrl.TrimEnd('/') + HUBURL;
        }

        private bool started = false;

        public async Task StartAsync()
        {
            if (!started)
            {
                hubConnection = new HubConnectionBuilder()
                    .WithUrl(hubUrl)
                    .Build();

                hubConnection.On<Guid, string>(SuspensionsHubMethods.NEWSUSPENSION, HandleNewSuspensionEvent);
                hubConnection.On<Guid, string>(SuspensionsHubMethods.SUSPENSIONUPDATED, HandleUpdatedSuspensionEvent);

                await hubConnection.StartAsync().ConfigureAwait(false);

                started = true;
            }
        }

        public delegate void SuspensionUpdatedEventHandler(object sender, SuspensionUpdatedEventArgs args);
        public event EventHandler<SuspensionUpdatedEventArgs> OnSuspensionUpdated;

        private void HandleUpdatedSuspensionEvent(Guid suspensionId, string channelOfOrigin)
        {
            OnSuspensionUpdated?.Invoke(this, new SuspensionUpdatedEventArgs { SuspensionId = suspensionId, ChannelOfOrigin = channelOfOrigin });
        }

        public delegate void NewSuspensionEventHandler(object sender, NewSuspensionEventArgs args);
        public event EventHandler<NewSuspensionEventArgs> OnNewSuspension;

        private void HandleNewSuspensionEvent(Guid suspensionId, string channelOfOrigin)
        {
            OnNewSuspension?.Invoke(this, new NewSuspensionEventArgs { SuspensionId = suspensionId, ChannelOfOrigin = channelOfOrigin });
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
