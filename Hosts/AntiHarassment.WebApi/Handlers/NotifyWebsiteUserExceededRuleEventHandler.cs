using AntiHarassment.Core;
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
    public class NotifyWebsiteUserExceededRuleEventHandler : IHandleMessages<NotifyWebsiteUserExceededRuleEvent>
    {
        private readonly INotificationService notificationService;
        private readonly IHubContext<NotificationHub> hubContext;

        public NotifyWebsiteUserExceededRuleEventHandler(INotificationService notificationService, IHubContext<NotificationHub> hubContext)
        {
            this.notificationService = notificationService;
            this.hubContext = hubContext;
        }

        public async Task Handle(NotifyWebsiteUserExceededRuleEvent message, IMessageHandlerContext context)
        {
            var connectionIdsForNotification = await notificationService.GetRecipientsFor(message.ChannelOfOrigin).ConfigureAwait(false);
            await hubContext.Clients.Clients(connectionIdsForNotification).SendAsync(NotificationHubMethods.NOTIFY, message.Username, message.RuleName, message.ChannelOfOrigin).ConfigureAwait(false);
        }
    }
}
