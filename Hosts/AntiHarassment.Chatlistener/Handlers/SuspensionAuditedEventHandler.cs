using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Messaging.Events;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Handlers
{
    public class SuspensionAuditedEventHandler : IHandleMessages<SuspensionAuditedEvent>
    {
        private readonly IAuditActionService auditActionService;
        private readonly ILogger<SuspensionAuditedEventHandler> logger;

        public SuspensionAuditedEventHandler(IAuditActionService auditActionService, ILogger<SuspensionAuditedEventHandler> logger)
        {
            this.auditActionService = auditActionService;
            this.logger = logger;
        }

        public async Task Handle(SuspensionAuditedEvent @event, IMessageHandlerContext context)
        {
            logger.LogInformation("Received audited event on suspension with id {arg}", @event.SuspensionId);

            await auditActionService.ReactTo(@event).ConfigureAwait(false);
        }
    }
}
