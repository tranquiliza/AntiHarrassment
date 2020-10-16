using AntiHarassment.Chatlistener.Core.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public interface IPubSubClient : IDisposable
    {
        event EventHandler<MessageReceivedEvent> OnMessageReceived;
        event EventHandler<MessageDeletedEvent> OnMessageDeleted;
        event EventHandler<UserTimedoutEvent> OnUserTimedout;
        event EventHandler<UserUntimedoutEvent> OnUserUnTimedout;
        event EventHandler<UserBannedEvent> OnUserBanned;
        event EventHandler<UserUnbannedEvent> OnUserUnbanned;

        Task<bool> JoinChannels(List<string> channelNames);
        Task<bool> JoinChannel(string channelName);
        bool LeaveChannel(string channelName);
        void Connect();
        void Disconnect();
    }
}
