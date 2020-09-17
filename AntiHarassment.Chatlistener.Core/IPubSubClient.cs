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

        Task<bool> JoinChannels(List<string> channelNames);
        Task<bool> JoinChannel(string channelName);
        bool LeaveChannel(string channelName);
        void Connect();
        void Disconnect();
    }
}
