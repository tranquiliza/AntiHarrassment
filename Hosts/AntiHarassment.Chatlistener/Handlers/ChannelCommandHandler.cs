using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Messaging.Commands;
using AntiHarassment.Messaging.Events;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Handlers
{
    public class ChannelCommandHandler
        : IHandleMessages<JoinChannelCommand>,
        IHandleMessages<LeaveChannelCommand>
    {
        private readonly IChatlistenerService chatlistenerService;

        public ChannelCommandHandler(IChatlistenerService chatlistenerService)
        {
            this.chatlistenerService = chatlistenerService;
        }

        public async Task Handle(JoinChannelCommand message, IMessageHandlerContext context)
        {
            await chatlistenerService.ListenTo(message.ChannelName).ConfigureAwait(false);

            await context.Publish(new JoinedChannelEvent(message.ChannelName)).ConfigureAwait(false);
        }

        public async Task Handle(LeaveChannelCommand message, IMessageHandlerContext context)
        {
            await chatlistenerService.UnlistenTo(message.ChannelName).ConfigureAwait(false);

            await context.Publish(new LeftChannelEvent(message.ChannelName)).ConfigureAwait(false);
        }
    }
}
