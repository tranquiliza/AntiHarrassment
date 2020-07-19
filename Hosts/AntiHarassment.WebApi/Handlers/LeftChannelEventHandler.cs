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
    public class LeftChannelEventHandler : IHandleMessages<LeftChannelEvent>
    {
        private readonly IHubContext<ChannelsHub> channelsHub;

        public LeftChannelEventHandler(IHubContext<ChannelsHub> channelsHub)
        {
            this.channelsHub = channelsHub;
        }

        public async Task Handle(LeftChannelEvent message, IMessageHandlerContext context)
        {
            await channelsHub.Clients.All.SendAsync(ChannelsHubMethods.CHANNELLEFT, message.ChannelName).ConfigureAwait(false);
        }
    }
}
