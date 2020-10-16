using System;

namespace AntiHarassment.Chatlistener.Core
{
    public interface IChannelMonitoringService : IDisposable
    {
        void Start();
    }
}
