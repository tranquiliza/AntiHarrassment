using AntiHarassment.Messaging.Events;
using AntiHarassment.SignalR.Contract;
using AntiHarassment.WebApi.Hubs;
using Microsoft.AspNetCore.SignalR;
using NServiceBus;
using System.Threading.Tasks;

namespace AntiHarassment.WebApi.Handlers
{
    public class ChannelEventHandler : IHandleMessages<JoinedChannelEvent>
        , IHandleMessages<LeftChannelEvent>
        , IHandleMessages<AutoModListenerDisabledForChannelEvent>
        , IHandleMessages<AutoModListenerEnabledForChannelEvent>
    {
        private readonly IHubContext<ChannelsHub> channelsHub;

        public ChannelEventHandler(IHubContext<ChannelsHub> channelsHub)
        {
            this.channelsHub = channelsHub;
        }

        public async Task Handle(JoinedChannelEvent message, IMessageHandlerContext context)
        {
            await channelsHub.Clients.All.SendAsync(ChannelsHubMethods.CHANNELJOINED, message.ChannelName).ConfigureAwait(false);
        }

        public async Task Handle(LeftChannelEvent message, IMessageHandlerContext context)
        {
            await channelsHub.Clients.All.SendAsync(ChannelsHubMethods.CHANNELLEFT, message.ChannelName).ConfigureAwait(false);
        }

        public async Task Handle(AutoModListenerEnabledForChannelEvent message, IMessageHandlerContext context)
        {
            await channelsHub.Clients.All.SendAsync(ChannelsHubMethods.AUTOMODLISTENERENABLED, message.ChannelName).ConfigureAwait(false);
        }

        public async Task Handle(AutoModListenerDisabledForChannelEvent message, IMessageHandlerContext context)
        {
            await channelsHub.Clients.All.SendAsync(ChannelsHubMethods.AUTOMODLISTENERDISABLED, message.ChannelName).ConfigureAwait(false);
        }
    }
}
