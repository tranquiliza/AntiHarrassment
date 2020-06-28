using AntiHarassment.Chatlistener.Core.Events;
using AntiHarassment.Chatlistener.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
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

        private async Task Client_OnUserTimedout(object _, UserTimedoutEvent e)
        {
            var suspension = Suspension.CreateTimeout(e.Username, e.Channel, e.TimeoutDuration, datetimeProvider.UtcNow);
            await suspensionRepository.SaveSuspension(suspension).ConfigureAwait(false);
        }

        private async Task Client_OnUserBanned(object _, UserBannedEvent e)
        {
            var suspension = Suspension.CreateBan(e.Username, e.Channel, datetimeProvider.UtcNow);
            await suspensionRepository.SaveSuspension(suspension).ConfigureAwait(false);
        }

        public async Task ConnectAndJoinChannels()
        {
            await client.Connect().ConfigureAwait(false);

            var channels = await channelRepository.GetChannels().ConfigureAwait(false);
            foreach (var channel in channels.Where(x => x.ShouldListen))
                await client.JoinChannel(channel.ChannelName).ConfigureAwait(false);
        }
    }
}
