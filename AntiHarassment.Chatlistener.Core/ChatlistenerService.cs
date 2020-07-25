using AntiHarassment.Chatlistener.Core.Events;
using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using AntiHarassment.Messaging.Events;
using AntiHarassment.Messaging.NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public class ChatlistenerService : IChatlistenerService
    {
        private TimeSpan ChatRecordTime = TimeSpan.FromMinutes(10);

        private readonly IChatClient client;
        private readonly IChannelRepository channelRepository;
        private readonly IDatetimeProvider datetimeProvider;
        private readonly ISuspensionRepository suspensionRepository;
        private readonly IChatRepository chatRepository;
        private readonly IServiceProvider serviceProvider;

        public ChatlistenerService(
            IChatClient client,
            IChannelRepository channelRepository,
            IDatetimeProvider datetimeProvider,
            ISuspensionRepository suspensionRepository,
            IChatRepository chatRepository,
            IServiceProvider serviceProvider)
        {
            this.client = client;
            this.channelRepository = channelRepository;
            this.datetimeProvider = datetimeProvider;
            this.suspensionRepository = suspensionRepository;
            this.chatRepository = chatRepository;
            this.serviceProvider = serviceProvider;

            client.OnUserBanned += async (sender, eventArgs) => await Client_OnUserBanned(sender, eventArgs).ConfigureAwait(false);
            client.OnUserTimedout += async (sender, eventArgs) => await Client_OnUserTimedout(sender, eventArgs).ConfigureAwait(false);

            client.OnMessageReceived += async (sender, eventArgs) => await Client_OnMessageReceived(sender, eventArgs).ConfigureAwait(false);
        }

        private async Task Client_OnMessageReceived(object _, MessageReceivedEvent e)
        {
            await chatRepository.SaveChatMessage(e.DisplayName, e.Channel, e.Message, datetimeProvider.UtcNow).ConfigureAwait(false);
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

            var timeOfSuspension = datetimeProvider.UtcNow;
            var chatlogForUser = await chatRepository.GetMessagesFor(e.Username, e.Channel, ChatRecordTime, timeOfSuspension).ConfigureAwait(false);
            var suspension = Suspension.CreateBan(e.Username, e.Channel, timeOfSuspension, chatlogForUser);
            await suspensionRepository.Save(suspension).ConfigureAwait(false);
            await messageDispatcher.Publish(new NewSuspensionEvent { SuspensionId = suspension.SuspensionId, ChannelOfOrigin = e.Channel }).ConfigureAwait(false);
        }

        public async Task ConnectAndJoinChannels()
        {
            await client.Connect().ConfigureAwait(false);

            var channels = await channelRepository.GetChannels().ConfigureAwait(false);
            foreach (var channel in channels.Where(x => x.ShouldListen))
                await client.JoinChannel(channel.ChannelName).ConfigureAwait(false);
        }

        public async Task ListenTo(string channelName)
        {
            var channel = await channelRepository.GetChannel(channelName).ConfigureAwait(false);
            if (channel == null)
                channel = new Channel(channelName, shouldListen: true);

            channel.EnableListening();

            await client.JoinChannel(channelName).ConfigureAwait(false);
            await channelRepository.Upsert(channel).ConfigureAwait(false);
        }

        public async Task UnlistenTo(string channelName)
        {
            var channel = await channelRepository.GetChannel(channelName).ConfigureAwait(false);
            if (channel == null)
                channel = new Channel(channelName, shouldListen: false);

            channel.DisableListening();

            await client.LeaveChannel(channelName).ConfigureAwait(false);
            await channelRepository.Upsert(channel).ConfigureAwait(false);
        }
    }
}
