using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Chatlistener.Core
{
    public interface IDatetimeProvider
    {
        DateTime UtcNow { get; }
    }
}
