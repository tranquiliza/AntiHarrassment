using AntiHarassment.Chatlistener.Core.Events;
using System;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public interface ICompositeChatClient : IDisposable
    {
        event Func<MessageReceivedEvent, Task> OnMessageReceived;
        event Func<UserJoinedEvent, Task> OnUserJoined;
        event Func<UserTimedoutEvent, Task> OnUserTimedOut;
        event Func<UserBannedEvent, Task> OnUserBanned;
        event Func<UserUnbannedEvent, Task> OnUserUnBanned;
        event Func<UserUntimedoutEvent, Task> OnUserUnTimedout;

        void SubscribeToEvents();
    }
}
