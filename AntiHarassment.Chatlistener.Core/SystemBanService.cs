using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using AntiHarassment.Messaging.Events;
using AntiHarassment.Messaging.NServiceBus;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public class SystemBanService : ISystemBanService
    {
        private readonly ISuspensionRepository suspensionRepository;
        private readonly IDatetimeProvider datetimeProvider;
        private readonly IChatClient client;
        private readonly IMessageDispatcher messageDispatcher;
        private readonly ILogger<SystemBanService> logger;

        public SystemBanService(ISuspensionRepository suspensionRepository, IDatetimeProvider datetimeProvider, IChatClient client, IMessageDispatcher messageDispatcher, ILogger<SystemBanService> logger)
        {
            this.suspensionRepository = suspensionRepository;
            this.datetimeProvider = datetimeProvider;
            this.client = client;
            this.messageDispatcher = messageDispatcher;
            this.logger = logger;
        }

        public async Task IssueBanFor(string username, string channelToBanFrom, string systemReason)
        {
            var suspensionsForUser = await suspensionRepository.GetSuspensionsForUser(username).ConfigureAwait(false);
            var suspensionsForUserInChannel = suspensionsForUser.Where(x => string.Equals(x.ChannelOfOrigin, channelToBanFrom, StringComparison.OrdinalIgnoreCase));

            if (suspensionsForUserInChannel.Any(x => x.SuspensionType == SuspensionType.Ban && !x.InvalidSuspension && x.Audited))
            {
                logger.LogInformation("{arg} has already been banned", username);
                return;
            }

            var suspension = Suspension.CreateSystemBan(username, channelToBanFrom, datetimeProvider.UtcNow, systemReason);
            client.BanUser(username, channelToBanFrom, systemReason);

            await suspensionRepository.Save(suspension).ConfigureAwait(false);
            await messageDispatcher.Publish(new NewSuspensionEvent { SuspensionId = suspension.SuspensionId, ChannelOfOrigin = channelToBanFrom }).ConfigureAwait(false);
        }
    }
}
