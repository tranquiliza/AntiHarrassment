using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public class SystemBanService : ISystemBanService
    {
        private readonly ISuspensionRepository suspensionRepository;
        private readonly IChatClient client;
        private readonly ILogger<SystemBanService> logger;

        public SystemBanService(ISuspensionRepository suspensionRepository, IChatClient client, ILogger<SystemBanService> logger)
        {
            this.suspensionRepository = suspensionRepository;
            this.client = client;
            this.logger = logger;
        }

        public async Task IssueBanFor(string username, string channelToBanFrom, string systemReason)
        {
            var suspensionsForUser = await suspensionRepository.GetSuspensionsForUser(username).ConfigureAwait(false);
            var suspensionsForUserInChannel = suspensionsForUser.Where(x => string.Equals(x.ChannelOfOrigin, channelToBanFrom, StringComparison.OrdinalIgnoreCase));

            if (suspensionsForUserInChannel.Any(x => x.SuspensionType == SuspensionType.Ban && !x.InvalidSuspension && x.Audited))
            {
                logger.LogInformation("{arg} has already been banned from {arg2}", username, channelToBanFrom);
                return;
            }

            client.BanUser(username, channelToBanFrom, systemReason);
        }
    }
}
