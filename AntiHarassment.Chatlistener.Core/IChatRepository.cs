using AntiHarassment.Chatlistener.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public interface IChatRepository
    {
        Task SaveChatMessage(string username, string channelOfOrigin, string message, DateTime timestamp);
        Task<List<ChatMessage>> GetMessagesFor(string username, string channelOfOrigin, TimeSpan historySpan, DateTime timeOfSuspension);
    }
}
