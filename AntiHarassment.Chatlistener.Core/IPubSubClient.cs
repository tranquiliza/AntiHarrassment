using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public interface IPubSubClient : IDisposable
    {
        Task JoinChannels(List<string> channelNames);
        Task Connect();
    }
}
