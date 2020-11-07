using AntiHarassment.Chatlistener.Core.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public class CompositeChatClient : ICompositeChatClient
    {
        private readonly IChatClient chatClient;
        private readonly IPubSubClient pubSubClient;
        private readonly ILogger<CompositeChatClient> logger;

        public event Func<MessageReceivedEvent, Task> OnMessageReceived;

        public event Func<UserJoinedEvent, Task> OnUserJoined;

        public event Func<UserTimedoutEvent, Task> OnUserTimedOut;
        public event Func<UserBannedEvent, Task> OnUserBanned;
        public event Func<UserUnbannedEvent, Task> OnUserUnBanned;
        public event Func<UserUntimedoutEvent, Task> OnUserUnTimedout;

        public CompositeChatClient(IChatClient chatClient, IPubSubClient pubSubClient, ILogger<CompositeChatClient> logger)
        {
            this.chatClient = chatClient;
            this.pubSubClient = pubSubClient;
            this.logger = logger;
        }

        public void SubscribeToEvents()
        {
            logger.LogInformation("Subscribing to events from Twitch");

            chatClient.OnMessageReceived += Clients_OnMessageReceived;
            pubSubClient.OnMessageReceived += Clients_OnMessageReceived;

            pubSubClient.OnMessageDeleted += PubSubClient_OnMessageDeleted;

            chatClient.OnUserBanned += ChatClient_OnUserBanned;
            pubSubClient.OnUserBanned += PubSub_OnUserBanned;

            chatClient.OnUserTimedout += Client_OnUserTimedout;
            pubSubClient.OnUserTimedout += PubSub_OnUserTimedout;

            chatClient.OnUserJoined += ChatClient_OnUserJoined;

            pubSubClient.OnUserUnbanned += PubSubClient_OnUserUnbanned;
            pubSubClient.OnUserUnTimedout += PubSubClient_OnUserUnTimedout;

            logger.LogInformation("Subscribing to events from Twitch COMPLETE");
        }

        private void PubSubClient_OnUserUnTimedout(object sender, UserUntimedoutEvent e)
            => OnUserUnTimedout?.Invoke(e);

        private void PubSubClient_OnUserUnbanned(object sender, UserUnbannedEvent e)
            => OnUserUnBanned?.Invoke(e);

        private void PubSubClient_OnMessageDeleted(object sender, MessageDeletedEvent e)
        {
            var messageReceivedEvent = new MessageReceivedEvent()
            {
                AutoModded = false,
                Channel = e.Channel,
                Deleted = true,
                DisplayName = e.Username,
                Message = e.Message,
                DeletedBy = e.DeletedBy,
                TwitchMessageId = e.TwitchMessageId
            };

            OnMessageReceived?.Invoke(messageReceivedEvent);
        }

        private void Client_OnUserTimedout(object sender, UserTimedoutEvent e)
        {
            e.Source = EventSource.IRC;
            PublishUserTimedoutEvent(e);
        }

        private void PubSub_OnUserTimedout(object sender, UserTimedoutEvent e)
        {
            e.Source = EventSource.PubSub;
            PublishUserTimedoutEvent(e);
        }

        private void PublishUserTimedoutEvent(UserTimedoutEvent e) => OnUserTimedOut?.Invoke(e);

        private void ChatClient_OnUserJoined(object sender, UserJoinedEvent e)
            => OnUserJoined?.Invoke(e);

        private void PubSub_OnUserBanned(object sender, UserBannedEvent e)
        {
            e.Source = EventSource.PubSub;
            PublishUserBannedEvent(e);
        }

        private void ChatClient_OnUserBanned(object sender, UserBannedEvent e)
        {
            e.Source = EventSource.IRC;
            PublishUserBannedEvent(e);
        }

        private void PublishUserBannedEvent(UserBannedEvent e) => OnUserBanned?.Invoke(e);


        private void Clients_OnMessageReceived(object sender, MessageReceivedEvent e)
            => OnMessageReceived?.Invoke(e);

        public void Dispose()
        {
            chatClient.OnMessageReceived -= Clients_OnMessageReceived;
            pubSubClient.OnMessageReceived -= Clients_OnMessageReceived;

            pubSubClient.OnMessageDeleted -= PubSubClient_OnMessageDeleted;

            chatClient.OnUserBanned -= ChatClient_OnUserBanned;
            pubSubClient.OnUserBanned -= ChatClient_OnUserBanned;

            chatClient.OnUserTimedout -= Client_OnUserTimedout;
            pubSubClient.OnUserTimedout -= Client_OnUserTimedout;

            chatClient.OnUserJoined -= ChatClient_OnUserJoined;

            pubSubClient.OnUserUnbanned -= PubSubClient_OnUserUnbanned;
            pubSubClient.OnUserUnTimedout -= PubSubClient_OnUserUnTimedout;
        }
    }
}
