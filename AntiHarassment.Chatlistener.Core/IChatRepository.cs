using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public interface IChatRepository
    {
        Task SaveChatMessage(string username, string channelOfOrigin, string message, DateTime timestamp);
    }
}
