using AntiHarassment.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public interface IChatRepository
    {
        Task SaveChatMessage(string username, string channelOfOrigin, bool autoModded, string message, DateTime timestamp);
        Task<List<ChatMessage>> GetMessagesFor(string username, string channelOfOrigin, TimeSpan historySpan, DateTime timeOfSuspension);
        Task<List<ChatMessage>> GetMessagesForChannel(string channelOfOrigin, DateTime earliestTime, DateTime latestTime);
        Task<List<string>> GetUniqueChattersForChannel(string channelOfOrigin);
        Task<List<string>> GetUniqueChattersForSystem();
        Task<DateTime> GetTimeStampForLatestMessage();
    }
}
