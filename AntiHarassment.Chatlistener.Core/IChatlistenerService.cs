using AntiHarassment.Core.Security;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public interface IChatlistenerService
    {
        Task ConnectAndJoinChannels();
        Task ListenTo(string channelName, IApplicationContext context);
        Task UnlistenTo(string channelName, IApplicationContext context);
        Task<bool> CheckConnectionAndRestartIfNeeded();
        Task JoinPubSub(string channelName, IApplicationContext context);
        Task LeavePubSub(string channelName, IApplicationContext context);
    }
}
