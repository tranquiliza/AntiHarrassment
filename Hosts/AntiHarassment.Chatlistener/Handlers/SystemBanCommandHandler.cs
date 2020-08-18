using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Messaging.Commands;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Handlers
{
    public class SystemBanCommandHandler : IHandleMessages<SystemBanCommand>
    {
        private readonly ISystemBanService systemBanService;

        public SystemBanCommandHandler(ISystemBanService systemBanService)
        {
            this.systemBanService = systemBanService;
        }

        public Task Handle(SystemBanCommand message, IMessageHandlerContext context)
        {
            return systemBanService.IssueBanFor(message.Username, message.ChannelToBanFrom, message.SystemReason);
        }
    }
}
