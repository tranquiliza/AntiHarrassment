using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using AntiHarassment.Messaging.Events;
using AntiHarassment.Messaging.NServiceBus;
using System;
using System.Collections.Generic;
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

        public SystemBanService(ISuspensionRepository suspensionRepository, IDatetimeProvider datetimeProvider, IChatClient client, IMessageDispatcher messageDispatcher)
        {
            this.suspensionRepository = suspensionRepository;
            this.datetimeProvider = datetimeProvider;
            this.client = client;
            this.messageDispatcher = messageDispatcher;
        }

        public async Task IssueBanFor(string username, string channelToBanFrom, string systemReason)
        {
            var suspension = Suspension.CreateSystemBan(username, channelToBanFrom, datetimeProvider.UtcNow, systemReason);
            client.BanUser(username, channelToBanFrom, systemReason);

            await suspensionRepository.Save(suspension).ConfigureAwait(false);
            await messageDispatcher.Publish(new NewSuspensionEvent { SuspensionId = suspension.SuspensionId, ChannelOfOrigin = channelToBanFrom }).ConfigureAwait(false);
        }
    }
}
