using AntiHarassment.Chatlistener.Core.Events;
using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public class ChatlistenerService : IChatlistenerService
    {
        private readonly IChatClient client;
        private readonly IChannelRepository channelRepository;
        private readonly IDatetimeProvider datetimeProvider;
        private readonly ISuspensionRepository suspensionRepository;
        private readonly IChatRepository chatRepository;

        public ChatlistenerService(
            IChatClient client,
            IChannelRepository channelRepository,
            IDatetimeProvider datetimeProvider,
            ISuspensionRepository suspensionRepository,
            IChatRepository chatRepository)
        {
            this.client = client;
            this.channelRepository = channelRepository;
            this.datetimeProvider = datetimeProvider;
            this.suspensionRepository = suspensionRepository;
            this.chatRepository = chatRepository;

            client.OnUserBanned += async (sender, eventArgs) => await Client_OnUserBanned(sender, eventArgs).ConfigureAwait(false);
            client.OnUserTimedout += async (sender, eventArgs) => await Client_OnUserTimedout(sender, eventArgs).ConfigureAwait(false);

            client.OnMessageReceived += async (sender, eventArgs) => await Client_OnMessageReceived(sender, eventArgs).ConfigureAwait(false);
        }

        private async Task Client_OnMessageReceived(object _, MessageReceivedEvent e)
        {
            await chatRepository.SaveChatMessage(e.DisplayName, e.Channel, e.Message, datetimeProvider.UtcNow).ConfigureAwait(false);
        }

        private TimeSpan ChatRecordTime = TimeSpan.FromMinutes(5);

        private async Task Client_OnUserTimedout(object _, UserTimedoutEvent e)
        {
            var timeOfSuspension = datetimeProvider.UtcNow;
            var chatlogForUser = await chatRepository.GetMessagesFor(e.Username, e.Channel, ChatRecordTime, timeOfSuspension).ConfigureAwait(false);
            var suspension = Suspension.CreateTimeout(e.Username, e.Channel, e.TimeoutDuration, timeOfSuspension, chatlogForUser);
            await suspensionRepository.SaveSuspension(suspension).ConfigureAwait(false);
        }

        private async Task Client_OnUserBanned(object _, UserBannedEvent e)
        {
            var timeOfSuspension = datetimeProvider.UtcNow;
            var chatlogForUser = await chatRepository.GetMessagesFor(e.Username, e.Channel, ChatRecordTime, timeOfSuspension).ConfigureAwait(false);
            var suspension = Suspension.CreateBan(e.Username, e.Channel, timeOfSuspension, chatlogForUser);
            await suspensionRepository.SaveSuspension(suspension).ConfigureAwait(false);
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
            await client.JoinChannel(channelName).ConfigureAwait(false);

            var channel = new Channel(channelName, shouldListen: true);
            await channelRepository.Upsert(channel).ConfigureAwait(false);
        }

        public async Task UnlistenTo(string channelName)
        {
            await client.LeaveChannel(channelName).ConfigureAwait(false);

            var channel = new Channel(channelName, shouldListen: false);
            await channelRepository.Upsert(channel).ConfigureAwait(false);
        }
    }
}
