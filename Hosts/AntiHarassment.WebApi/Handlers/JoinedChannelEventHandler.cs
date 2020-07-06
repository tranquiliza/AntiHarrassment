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
    public class JoinedChannelEventHandler : IHandleMessages<JoinedChannelEvent>
    {
        private readonly IHubContext<ChannelsHub> channelsHub;

        public JoinedChannelEventHandler(IHubContext<ChannelsHub> channelsHub)
        {
            this.channelsHub = channelsHub;
        }

        public async Task Handle(JoinedChannelEvent message, IMessageHandlerContext context)
        {
            await channelsHub.Clients.All.SendAsync(Methods.CHANNELJOINED, message.ChannelName).ConfigureAwait(false);
        }
    }
}
