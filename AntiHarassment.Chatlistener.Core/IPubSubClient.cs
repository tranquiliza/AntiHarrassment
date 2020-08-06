using AntiHarassment.Chatlistener.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public interface IPubSubClient : IDisposable
    {
        event EventHandler<MessageReceivedEvent> OnMessageReceived;

        Task<bool> JoinChannels(List<string> channelNames);
        Task<bool> JoinChannel(string channelName);
        bool LeaveChannel(string channelName);
        Task Connect();
        Task<bool> Disconnect();
    }
}
