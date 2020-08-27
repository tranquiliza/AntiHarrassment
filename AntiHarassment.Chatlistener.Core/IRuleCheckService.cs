using AntiHarassment.Messaging.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public interface IRuleCheckService
    {
        Task ReactTo(RuleCheckFromAuditCommand command);
        Task CheckRulesFor(string username, string channelName);
    }
}
