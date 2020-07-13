using AntiHarassment.Core.Models;
using AntiHarassment.Core.Security;
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

        public async Task<IResult<Channel>> GetChannel(string channelName, IApplicationContext context)
        {
            if (!string.Equals(context.User.TwitchUsername, channelName, StringComparison.OrdinalIgnoreCase) || !context.User.HasRole(Roles.Admin))
                return Result<Channel>.Unauthorized();

            var result = await channelRepository.GetChannel(channelName).ConfigureAwait(false);
            if (result == null)
                return Result<Channel>.NoContentFound();

            return Result<Channel>.Succeeded(result);
        }

        public async Task<IResult<List<Channel>>> GetChannels(IApplicationContext context)
        {
            if (!context.User.HasRole(Roles.Admin))
                return Result<List<Channel>>.Unauthorized();

            var result = await channelRepository.GetChannels().ConfigureAwait(false);
            if (result.Count == 0)
                return Result<List<Channel>>.NoContentFound();

            return Result<List<Channel>>.Succeeded(result);
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
