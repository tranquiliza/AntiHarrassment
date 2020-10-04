using System;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public interface IDiscordNotificationService
    {
        Task SendNotification(string username, string channelOfOrigin, Guid ruleId);
    }
}