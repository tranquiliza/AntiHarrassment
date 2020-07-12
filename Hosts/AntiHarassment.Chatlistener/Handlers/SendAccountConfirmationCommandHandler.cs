using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Messaging.Commands;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Handlers
{
    public class SendAccountConfirmationCommandHandler : IHandleMessages<SendAccountConfirmationCommand>
    {
        private readonly IUserNotificationService userNotificationService;

        public SendAccountConfirmationCommandHandler(IUserNotificationService userNotificationService)
        {
            this.userNotificationService = userNotificationService;
        }

        public Task Handle(SendAccountConfirmationCommand message, IMessageHandlerContext context)
        {
            return userNotificationService.SendConfirmationTokenToUser(message.UserId);
        }
    }
}
