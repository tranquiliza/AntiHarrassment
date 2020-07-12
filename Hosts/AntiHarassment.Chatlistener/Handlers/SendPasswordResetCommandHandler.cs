using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Messaging.Commands;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Handlers
{
    public class SendPasswordResetCommandHandler : IHandleMessages<SendPasswordResetTokenCommand>
    {
        private readonly IUserNotificationService userNotificationService;

        public SendPasswordResetCommandHandler(IUserNotificationService userNotificationService)
        {
            this.userNotificationService = userNotificationService;
        }

        public Task Handle(SendPasswordResetTokenCommand message, IMessageHandlerContext context)
        {
            return userNotificationService.SendPassworkResetTokenToUser(message.UserId);
        }
    }
}
