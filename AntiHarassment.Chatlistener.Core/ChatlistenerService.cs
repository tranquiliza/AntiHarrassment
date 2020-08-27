using AntiHarassment.Chatlistener.Core.Events;
using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using AntiHarassment.Core.Security;
using AntiHarassment.Messaging.Events;
using AntiHarassment.Messaging.NServiceBus;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public class ChatlistenerService : IChatlistenerService, IDisposable
    {
        private TimeSpan ChatRecordTime = TimeSpan.FromMinutes(10);

        private readonly IChatClient client;
        private readonly IPubSubClient pubSubClient;
        private readonly IChannelRepository channelRepository;
        private readonly IDatetimeProvider datetimeProvider;
        private readonly ISuspensionRepository suspensionRepository;
        private readonly IChatRepository chatRepository;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<ChatlistenerService> logger;

        public ChatlistenerService(
            IChatClient client,
            IPubSubClient pubSubClient,
            IChannelRepository channelRepository,
            IDatetimeProvider datetimeProvider,
            ISuspensionRepository suspensionRepository,
            IChatRepository chatRepository,
            IServiceProvider serviceProvider,
            ILogger<ChatlistenerService> logger)
        {
            this.client = client;
            this.pubSubClient = pubSubClient;
            this.channelRepository = channelRepository;
            this.datetimeProvider = datetimeProvider;
            this.suspensionRepository = suspensionRepository;
            this.chatRepository = chatRepository;
            this.serviceProvider = serviceProvider;
            this.logger = logger;

            client.OnUserBanned += async (sender, eventArgs) => await Client_OnUserBanned(sender, eventArgs).ConfigureAwait(false);
            client.OnUserTimedout += async (sender, eventArgs) => await Client_OnUserTimedout(sender, eventArgs).ConfigureAwait(false);

            client.OnMessageReceived += async (sender, eventArgs) => await OnMessageReceived(sender, eventArgs).ConfigureAwait(false);
            pubSubClient.OnMessageReceived += async (sender, eventArgs) => await OnMessageReceived(sender, eventArgs).ConfigureAwait(false);
        }

        private async Task OnMessageReceived(object _, MessageReceivedEvent e)
        {
            await chatRepository.SaveChatMessage(e.DisplayName, e.Channel, e.AutoModded, e.Message, datetimeProvider.UtcNow).ConfigureAwait(false);
        }

        private async Task Client_OnUserTimedout(object _, UserTimedoutEvent e)
        {
            if (e.TimeoutDuration <= 10)
                return;

            var messageDispatcher = serviceProvider.GetService(typeof(IMessageDispatcher)) as IMessageDispatcher;

            var timeOfSuspension = datetimeProvider.UtcNow;
            var chatlogForUser = await chatRepository.GetMessagesFor(e.Username, e.Channel, ChatRecordTime, timeOfSuspension).ConfigureAwait(false);
            var suspension = Suspension.CreateTimeout(e.Username, e.Channel, e.TimeoutDuration, timeOfSuspension, chatlogForUser);
            await suspensionRepository.Save(suspension).ConfigureAwait(false);
            await messageDispatcher.Publish(new NewSuspensionEvent { SuspensionId = suspension.SuspensionId, ChannelOfOrigin = e.Channel }).ConfigureAwait(false);
        }

        private async Task Client_OnUserBanned(object _, UserBannedEvent e)
        {
            var messageDispatcher = serviceProvider.GetService(typeof(IMessageDispatcher)) as IMessageDispatcher;

            var suspensionsForUser = await suspensionRepository.GetSuspensionsForUser(e.Username).ConfigureAwait(false);
            var latestForChannel = suspensionsForUser
                .Where(x => string.Equals(x.ChannelOfOrigin, e.Channel, StringComparison.OrdinalIgnoreCase) && x.SuspensionType == SuspensionType.Ban)
                .OrderByDescending(x => x.Timestamp).FirstOrDefault();

            var timeOfSuspension = datetimeProvider.UtcNow;
            if (latestForChannel != null && latestForChannel.Timestamp.AddSeconds(30) >= timeOfSuspension)
                return;

            var chatlogForUser = await chatRepository.GetMessagesFor(e.Username, e.Channel, ChatRecordTime, timeOfSuspension).ConfigureAwait(false);
            var suspension = Suspension.CreateBan(e.Username, e.Channel, timeOfSuspension, chatlogForUser);
            await suspensionRepository.Save(suspension).ConfigureAwait(false);
            await messageDispatcher.Publish(new NewSuspensionEvent { SuspensionId = suspension.SuspensionId, ChannelOfOrigin = e.Channel }).ConfigureAwait(false);
        }

        private bool hasBootedUp = false;

        public async Task<bool> CheckConnectionAndRestartIfNeeded()
        {
            if (!hasBootedUp)
                return false;

            var timeOfLatestMessage = await chatRepository.GetTimeStampForLatestMessage().ConfigureAwait(false);
            var timeOfCheck = datetimeProvider.UtcNow;
            logger.LogInformation("time of latest message: {arg}, time of check: {argTwo}", timeOfLatestMessage, timeOfCheck);
            if (timeOfLatestMessage < timeOfCheck.AddMinutes(-30))
            {
                await client.Disconnect().ConfigureAwait(false);
                logger.LogInformation("Client Disconnected");

                await Task.Delay(TimeSpan.FromMinutes(1)).ConfigureAwait(false);

                await client.Connect().ConfigureAwait(false);
                var channels = await channelRepository.GetChannels().ConfigureAwait(false);
                foreach (var channel in channels.Where(x => x.ShouldListen))
                    await client.JoinChannel(channel.ChannelName).ConfigureAwait(false);

                logger.LogInformation("Client Reconnected and Joined");

                return true;
            }

            return false;
        }

        public async Task ConnectAndJoinChannels()
        {
            logger.LogInformation("Connecting and joining channels");
            await client.Connect().ConfigureAwait(false);
            await pubSubClient.Connect().ConfigureAwait(false);

            var channels = await channelRepository.GetChannels().ConfigureAwait(false);
            var enabledChannels = channels.Where(x => x.ShouldListen);

            if (!await pubSubClient.JoinChannels(enabledChannels.Where(x => x.SystemIsModerator).Select(x => x.ChannelName).ToList()).ConfigureAwait(false))
                logger.LogWarning("Unable to join all channels. Have we hit the channel cap? {enabledChannels}", enabledChannels.Count());

            foreach (var channel in enabledChannels)
                await client.JoinChannel(channel.ChannelName).ConfigureAwait(false);

            logger.LogInformation("Connected to channels");

            hasBootedUp = true;
        }

        public async Task ListenTo(string channelName, IApplicationContext context)
        {
            var channel = await channelRepository.GetChannel(channelName).ConfigureAwait(false);
            if (channel == null)
                channel = new Channel(channelName, shouldListen: true);

            channel.EnableListening(context, datetimeProvider.UtcNow);

            if (channel.SystemIsModerator)
            {
                if (await pubSubClient.JoinChannel(channelName).ConfigureAwait(false))
                    channel.EnableAutoModdedMessageListening(context, datetimeProvider.UtcNow);
                else
                    logger.LogWarning("Unable to join channel {channelName} have we hit the cap?", channelName);
            }

            await client.JoinChannel(channelName).ConfigureAwait(false);
            await channelRepository.Upsert(channel).ConfigureAwait(false);
        }

        public async Task UnlistenTo(string channelName, IApplicationContext context)
        {
            var channel = await channelRepository.GetChannel(channelName).ConfigureAwait(false);
            if (channel == null)
                channel = new Channel(channelName, shouldListen: false);

            channel.DisableListening(context, datetimeProvider.UtcNow);
            channel.DisableAutoModdedMessageListening(context, datetimeProvider.UtcNow);

            if (!pubSubClient.LeaveChannel(channelName))
                logger.LogInformation("Was unable to leave {channelName}", channelName);

            await client.LeaveChannel(channelName).ConfigureAwait(false);
            await channelRepository.Upsert(channel).ConfigureAwait(false);
        }

        public async Task JoinPubSub(string channelName, IApplicationContext context)
        {
            var channel = await channelRepository.GetChannel(channelName).ConfigureAwait(false);
            if (channel == null)
            {
                logger.LogWarning("Channel updated moderation state, but we cant find that channel??? {arg}", channelName);
                return;
            }

            if (channel.SystemIsModerator)
            {
                if (await pubSubClient.JoinChannel(channel.ChannelName).ConfigureAwait(false))
                {
                    channel.EnableAutoModdedMessageListening(context, datetimeProvider.UtcNow);
                    await channelRepository.Upsert(channel).ConfigureAwait(false);
                }
                else
                {
                    logger.LogWarning("Unable to connect pubsub for {channel}", channelName);
                }
            }
        }

        public async Task LeavePubSub(string channelName, IApplicationContext context)
        {
            var channel = await channelRepository.GetChannel(channelName).ConfigureAwait(false);
            if (channel == null)
            {
                logger.LogWarning("Channel updated moderation state, but we cant find that channel??? {arg}", channelName);
                return;
            }

            if (pubSubClient.LeaveChannel(channelName))
            {
                channel.DisableAutoModdedMessageListening(context, datetimeProvider.UtcNow);
                await channelRepository.Upsert(channel).ConfigureAwait(false);
            }
            else
            {
                logger.LogWarning("Unable to leave pubsub for channel {arg}", channelName);
            }
        }

        public void Dispose()
        {
            client.Dispose();
            pubSubClient.Dispose();
        }
    }
}
