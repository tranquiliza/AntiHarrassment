using AntiHarassment.Core.Models;
using AntiHarassment.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public class SuspensionService : ISuspensionService
    {
        private readonly ISuspensionRepository suspensionRepository;
        private readonly IChannelRepository channelRepository;

        public SuspensionService(ISuspensionRepository suspensionRepository, IChannelRepository channelRepository)
        {
            this.suspensionRepository = suspensionRepository;
            this.channelRepository = channelRepository;
        }

        public async Task<IResult<List<Suspension>>> GetAllSuspensionsAsync(string channelOfOrigin, IApplicationContext context)
        {
            var channel = await channelRepository.GetChannel(channelOfOrigin).ConfigureAwait(false);
            if (channel == null)
                return Result<List<Suspension>>.NoContentFound();

            if (!HaveAccess(context, channel))
                return Result<List<Suspension>>.Unauthorized();

            var dataForUser = await suspensionRepository.GetSuspensionsForChannel(channelOfOrigin).ConfigureAwait(false);
            if (dataForUser.Count > 0)
                return Result<List<Suspension>>.Succeeded(dataForUser.OrderByDescending(x => x.Timestamp).ToList());

            return Result<List<Suspension>>.NoContentFound();
        }

        public async Task<IResult<Suspension>> UpdateValidity(Guid suspensionId, bool invalidate, IApplicationContext context)
        {
            var suspension = await suspensionRepository.GetSuspension(suspensionId).ConfigureAwait(false);
            if (suspension == null)
                return Result<Suspension>.Failure($"Unable to find suspension, invalid Id?: {suspensionId}");

            var channel = await channelRepository.GetChannel(suspension.ChannelOfOrigin).ConfigureAwait(false);
            if (channel == null)
                return Result<Suspension>.Failure($"The {suspension.ChannelOfOrigin} channel, was not found");

            if (!HaveAccess(context, channel))
                return Result<Suspension>.Unauthorized();

            suspension.UpdateValidity(invalidate);
            await suspensionRepository.SaveSuspension(suspension).ConfigureAwait(false);

            return Result<Suspension>.Succeeded(suspension);
        }

        private bool HaveAccess(IApplicationContext context, Channel channel)
        {
            return string.Equals(context.User?.TwitchUsername, channel.ChannelName, StringComparison.OrdinalIgnoreCase)
                            || context.User?.HasRole(Roles.Admin) == true
                            || channel.HasModerator(context.User?.TwitchUsername);
        }
    }
}
