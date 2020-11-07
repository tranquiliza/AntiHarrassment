using System;

namespace AntiHarassment.Chatlistener.Core
{
    public interface ISuspensionLogService : IDisposable
    {
        void Start();
    }
}
