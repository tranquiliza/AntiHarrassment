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

            if (!HaveAccess())
                return Result<List<Suspension>>.Unauthorized();

            var dataForUser = await suspensionRepository.GetSuspensionsForChannel(channelOfOrigin).ConfigureAwait(false);
            if (dataForUser.Count > 0)
                return Result<List<Suspension>>.Succeeded(dataForUser.OrderByDescending(x => x.Timestamp).ToList());

            return Result<List<Suspension>>.NoContentFound();

            bool HaveAccess() => string.Equals(context.User?.TwitchUsername, channelOfOrigin, StringComparison.OrdinalIgnoreCase)
                            || context.User?.HasRole(Roles.Admin) == true
                            || channel.HasModerator(context.User?.TwitchUsername);
        }
    }
}
