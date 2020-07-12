using AntiHarassment.Core.Models;
using AntiHarassment.Messaging.Commands;
using AntiHarassment.Messaging.NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public class ChannelService : IChannelService
    {
        private readonly IChannelRepository channelRepository;
        private readonly IMessageDispatcher messageDispatcher;

        public ChannelService(IChannelRepository channelRepository, IMessageDispatcher messageDispatcher)
        {
            this.channelRepository = channelRepository;
            this.messageDispatcher = messageDispatcher;
        }

        public Task<List<Channel>> GetChannels()
        {
            return channelRepository.GetChannels();
        }

        public async Task UpdateChannel(string channelName, bool shouldListen)
        {
            if (shouldListen)
            {
                var joinChannelCommand = new JoinChannelCommand { ChannelName = channelName };
                await messageDispatcher.Send(joinChannelCommand).ConfigureAwait(false);
            }
            else
            {
                var leaveChannelCommand = new LeaveChannelCommand { ChannelName = channelName };
                await messageDispatcher.Send(leaveChannelCommand).ConfigureAwait(false);
            }
        }
    }
}
