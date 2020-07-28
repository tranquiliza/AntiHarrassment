using AntiHarassment.Core.Security;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public interface IChatlistenerService
    {
        Task ConnectAndJoinChannels();
        Task ListenTo(string channelName, IApplicationContext context);
        Task UnlistenTo(string channelName, IApplicationContext context);
    }
}
