using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Messaging.Commands;
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

        public Task Handle(JoinChannelCommand message, IMessageHandlerContext context)
        {
            return chatlistenerService.ListenTo(message.ChannelName);
        }

        public Task Handle(LeaveChannelCommand message, IMessageHandlerContext context)
        {
            return chatlistenerService.UnlistenTo(message.ChannelName);
        }
    }
}
