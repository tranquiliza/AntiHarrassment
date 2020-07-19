using AntiHarassment.Core.Models;
using AntiHarassment.Core.Security;
using AntiHarassment.Messaging.Commands;
using AntiHarassment.Messaging.NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IResult<Channel>> AddModeratorToChannel(string channelName, string moderatorTwitchUsername, IApplicationContext context)
        {
            if (!string.Equals(channelName, context.User.TwitchUsername))
                return Result<Channel>.Unauthorized();

            var channel = await channelRepository.GetChannel(channelName).ConfigureAwait(false);
            if (channel == null)
                return Result<Channel>.Failure("Channel was not found.");

            if (!channel.TryAddModerator(moderatorTwitchUsername))
                return Result<Channel>.Failure("Unable to add moderator");

            await channelRepository.Upsert(channel).ConfigureAwait(false);

            return Result<Channel>.Succeeded(channel);
        }

        public async Task<IResult<Channel>> DeleteModeratorFromChannel(string channelName, string moderatorTwitchUsername, IApplicationContext context)
        {
            if (!string.Equals(channelName, context.User.TwitchUsername))
                return Result<Channel>.Unauthorized();

            var channel = await channelRepository.GetChannel(channelName).ConfigureAwait(false);
            if (channel == null)
                return Result<Channel>.Failure("Channel was not found.");

            channel.RemoveModerator(moderatorTwitchUsername);

            await channelRepository.Upsert(channel).ConfigureAwait(false);

            return Result<Channel>.Succeeded(channel);
        }

        public async Task<IResult<Channel>> GetChannel(string channelName, IApplicationContext context)
        {
            if (!string.Equals(context.User.TwitchUsername, channelName, StringComparison.OrdinalIgnoreCase) && !context.User.HasRole(Roles.Admin))
                return Result<Channel>.Unauthorized();

            var result = await channelRepository.GetChannel(channelName).ConfigureAwait(false);
            if (result == null)
                return Result<Channel>.NoContentFound();

            return Result<Channel>.Succeeded(result);
        }

        public async Task<IResult<List<Channel>>> GetChannels(IApplicationContext context)
        {
            var query = await channelRepository.GetChannels().ConfigureAwait(false);
            if (context.User.HasRole(Roles.Admin))
                return Result<List<Channel>>.Succeeded(query);

            var result = query.Where(channel => channel.HasModerator(context.User.TwitchUsername)
            || string.Equals(channel.ChannelName, context.User.TwitchUsername, StringComparison.OrdinalIgnoreCase)).ToList();

            if (result.Count == 0)
                return Result<List<Channel>>.NoContentFound();

            return Result<List<Channel>>.Succeeded(result);
        }

        public async Task UpdateChannel(string channelName, bool shouldListen, IApplicationContext context)
        {
            if (!context.User.HasRole(Roles.Admin) && !string.Equals(context.User.TwitchUsername, channelName, StringComparison.OrdinalIgnoreCase))
                return;

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
