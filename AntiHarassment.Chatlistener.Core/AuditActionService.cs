using AntiHarassment.Messaging.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public class AuditActionService : IAuditActionService
    {
        public AuditActionService()
        {
        }

        public Task ReactTo(SuspensionAuditedEvent @event)
        {
            // Check if we even have any channels with feature enabled?
            // Grab all information about this user.
            // Create a UserReport? 

            throw new NotImplementedException();
        }
    }
}
