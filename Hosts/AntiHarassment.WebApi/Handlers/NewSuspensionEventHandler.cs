using AntiHarassment.Core.Models;
using AntiHarassment.Messaging.Events;
using AntiHarassment.SignalR.Contract;
using AntiHarassment.WebApi.Hubs;
using Microsoft.AspNetCore.SignalR;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.WebApi.Handlers
{
    public class NewSuspensionEventHandler : IHandleMessages<NewSuspensionEvent>, IHandleMessages<SuspensionUpdatedEvent>
    {
        private readonly IHubContext<SuspensionsHub> suspensionsHub;

        public NewSuspensionEventHandler(IHubContext<SuspensionsHub> suspensionsHub)
        {
            this.suspensionsHub = suspensionsHub;
        }

        public async Task Handle(NewSuspensionEvent message, IMessageHandlerContext context)
        {
            await suspensionsHub.Clients.All.SendAsync(SuspensionsHubMethods.NEWSUSPENSION, message.SuspensionId, message.ChannelOfOrigin).ConfigureAwait(false);
        }

        public async Task Handle(SuspensionUpdatedEvent message, IMessageHandlerContext context)
        {
            await suspensionsHub.Clients.All.SendAsync(SuspensionsHubMethods.SUSPENSIONUPDATED, message.SuspensionId, message.ChannelOfOrigin).ConfigureAwait(false);
        }
    }
}
