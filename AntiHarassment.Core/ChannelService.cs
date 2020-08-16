using AntiHarassment.Core.Models;
using AntiHarassment.Core.Security;
using AntiHarassment.Messaging.Commands;
using AntiHarassment.Messaging.Events;
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
        private readonly IChatRepository chatRepository;
        private readonly IDatetimeProvider datetimeProvider;

        public ChannelService(
            IChannelRepository channelRepository,
            IMessageDispatcher messageDispatcher,
            IChatRepository chatRepository,
            IDatetimeProvider datetimeProvider)
        {
            this.channelRepository = channelRepository;
            this.messageDispatcher = messageDispatcher;
            this.chatRepository = chatRepository;
            this.datetimeProvider = datetimeProvider;
        }

        public async Task<IResult<Channel>> AddModeratorToChannel(string channelName, string moderatorTwitchUsername, IApplicationContext context)
        {
            if (!string.Equals(channelName, context.User.TwitchUsername) && !context.User.HasRole(Roles.Admin))
                return Result<Channel>.Unauthorized();

            var channel = await channelRepository.GetChannel(channelName).ConfigureAwait(false);
            if (channel == null)
                return Result<Channel>.Failure("Channel was not found.");

            if (!channel.TryAddModerator(moderatorTwitchUsername, context, datetimeProvider.UtcNow))
                return Result<Channel>.Failure("Unable to add moderator");

            await channelRepository.Upsert(channel).ConfigureAwait(false);

            return Result<Channel>.Succeeded(channel);
        }

        public async Task<IResult<Channel>> DeleteModeratorFromChannel(string channelName, string moderatorTwitchUsername, IApplicationContext context)
        {
            if (!string.Equals(channelName, context.User.TwitchUsername) && !context.User.HasRole(Roles.Admin))
                return Result<Channel>.Unauthorized();

            var channel = await channelRepository.GetChannel(channelName).ConfigureAwait(false);
            if (channel == null)
                return Result<Channel>.Failure("Channel was not found.");

            channel.RemoveModerator(moderatorTwitchUsername, context, datetimeProvider.UtcNow);

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

        public async Task UpdateChannelListenerState(string channelName, bool shouldListen, IApplicationContext context)
        {
            if (!context.User.HasRole(Roles.Admin) && !string.Equals(context.User.TwitchUsername, channelName, StringComparison.OrdinalIgnoreCase))
                return;

            if (shouldListen)
            {
                var joinChannelCommand = new JoinChannelCommand { ChannelName = channelName, RequestedByUserId = context.UserId };
                await messageDispatcher.Send(joinChannelCommand).ConfigureAwait(false);
            }
            else
            {
                var leaveChannelCommand = new LeaveChannelCommand { ChannelName = channelName, RequestedByUserId = context.UserId };
                await messageDispatcher.Send(leaveChannelCommand).ConfigureAwait(false);
            }
        }

        public async Task<IResult<Channel>> UpdateChannelSystemIsModeratorState(string channelName, bool systemIsModerator, IApplicationContext context)
        {
            var channel = await channelRepository.GetChannel(channelName).ConfigureAwait(false);
            if (channel == null)
                return Result<Channel>.NoContentFound();

            if (!context.HaveOwnerAccessTo(channel))
                return Result<Channel>.Unauthorized();

            channel.UpdateSystemModerationStatus(systemIsModerator, context, datetimeProvider.UtcNow);
            await channelRepository.Upsert(channel).ConfigureAwait(false);

            await PublishChannelModerationChangedEvent(channel, context.UserId).ConfigureAwait(false);

            return Result<Channel>.Succeeded(channel);
        }

        private async Task PublishChannelModerationChangedEvent(Channel channel, Guid requestedByUserId)
        {
            var @event = new ChannelChangedSystemModerationEvent
            {
                ChannelName = channel.ChannelName,
                SystemIsModerator = channel.SystemIsModerator,
                RequestedByUserId = requestedByUserId
            };

            await messageDispatcher.Publish(@event).ConfigureAwait(false);
        }

        public async Task<IResult<List<ChatMessage>>> GetChatLogs(string channelName, DateTime earliestTime, DateTime latestDate, IApplicationContext context)
        {
            var channel = await channelRepository.GetChannel(channelName).ConfigureAwait(false);
            if (!context.HaveAccessTo(channel))
                return Result<List<ChatMessage>>.Unauthorized();

            var chatLogs = await chatRepository.GetMessagesForChannel(channel.ChannelName, earliestTime, latestDate).ConfigureAwait(false);
            if (chatLogs.Count == 0)
                return Result<List<ChatMessage>>.NoContentFound();

            return Result<List<ChatMessage>>.Succeeded(chatLogs);
        }

        public async Task<IResult<List<string>>> GetChattersForChannel(string channelName, IApplicationContext context)
        {
            var channel = await channelRepository.GetChannel(channelName).ConfigureAwait(false);
            if (!context.HaveAccessTo(channel))
                return Result<List<string>>.Unauthorized();

            var users = await chatRepository.GetUniqueChattersForChannel(channelName).ConfigureAwait(false);
            if (users.Count == 0)
                return Result<List<string>>.NoContentFound();

            return Result<List<string>>.Succeeded(users);
        }
    }
}
