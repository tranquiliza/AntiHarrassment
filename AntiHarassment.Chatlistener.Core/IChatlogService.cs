using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Chatlistener.Core
{
    public interface IChatlogService : IDisposable
    {
        void Start();
    }
}
