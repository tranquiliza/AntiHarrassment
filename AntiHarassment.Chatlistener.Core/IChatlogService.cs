using System;

namespace AntiHarassment.Chatlistener.Core
{
    public interface IChatlogService : IDisposable
    {
        void Start();
    }
}
