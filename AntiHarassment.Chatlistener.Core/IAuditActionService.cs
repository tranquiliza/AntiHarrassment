using AntiHarassment.Messaging.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public interface IAuditActionService
    {
        Task ReactTo(SuspensionAuditedEvent @event);
    }
}
