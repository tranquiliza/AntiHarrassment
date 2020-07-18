﻿using AntiHarassment.SignalR.Contract.EventArgs;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
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

                await hubConnection.StartAsync().ConfigureAwait(false);

                started = true;
            }
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
                await hubConnection.StopAsync();
                await hubConnection.DisposeAsync();
                hubConnection = null;
                started = false;
            }
        }
    }
}