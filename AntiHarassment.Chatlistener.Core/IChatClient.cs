using AntiHarassment.Chatlistener.Core.Events;
using System;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public interface IChatClient : IDisposable
    {
        event EventHandler<MessageReceivedEvent> OnMessageReceived;
        event EventHandler<UserJoinedEvent> OnUserJoined;
        event EventHandler<UserBannedEvent> OnUserBanned;
        event EventHandler<UserTimedoutEvent> OnUserTimedout;

        Task Connect();
        Task Disconnect();
        Task JoinChannel(string channelName);
        Task LeaveChannel(string channelName);
        Task SendWhisper(string channel, string message);
        void BanUser(string username, string channelName, string systemReason);
        Task Reconnect();
    }
}
