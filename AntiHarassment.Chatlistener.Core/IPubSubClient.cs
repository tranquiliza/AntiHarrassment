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

        Task JoinChannels(List<string> channelNames);
        Task Connect();
    }
}
