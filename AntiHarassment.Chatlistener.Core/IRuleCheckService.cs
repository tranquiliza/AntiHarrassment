using AntiHarassment.Messaging.Commands;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public interface IRuleCheckService
    {
        Task CheckBanAction(RuleExceedCheckCommand command);
        Task CheckRulesFor(string username, string channelName);
    }
}
