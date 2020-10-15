using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Chatlistener.Core
{
    public interface ISuspensionLogService : IDisposable
    {
        void Start();
    }
}
