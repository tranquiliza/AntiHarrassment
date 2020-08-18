using AntiHarassment.Core.Models;
using AntiHarassment.Core.Security;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public class ChannelReportService : IChannelReportService
    {
        private readonly IChannelRepository channelRepository;
        private readonly ISuspensionRepository suspensionRepository;
        private readonly IChatRepository chatRepository;
        private readonly IDatetimeProvider datetimeProvider;

        public ChannelReportService(
            IChannelRepository channelRepository,
            ISuspensionRepository suspensionRepository,
            IChatRepository chatRepository,
            IDatetimeProvider datetimeProvider)
        {
            this.channelRepository = channelRepository;
            this.suspensionRepository = suspensionRepository;
            this.chatRepository = chatRepository;
            this.datetimeProvider = datetimeProvider;
        }

        public async Task<IResult<ChannelReport>> GenerateReportForChannel(string channelName, IApplicationContext context)
        {
            var channel = await channelRepository.GetChannel(channelName).ConfigureAwait(false);
            if (!context.HaveAccessTo(channel))
                return Result<ChannelReport>.Unauthorized();

            var suspensionsForChannel = await suspensionRepository.GetAuditedSuspensionsForChannel(channelName, datetimeProvider.UtcNow.AddDays(-30)).ConfigureAwait(false);
            if (suspensionsForChannel.Count == 0)
                return Result<ChannelReport>.NoContentFound();

            var usersForChannel = await chatRepository.GetUniqueChattersForChannel(channelName).ConfigureAwait(false);

            var suspensionsForChannelWithoutSystem = suspensionsForChannel.Where(x => x.SuspensionSource != SuspensionSource.System).ToList();

            var channelReport = new ChannelReport(channelName, suspensionsForChannelWithoutSystem, usersForChannel.Count);
            return Result<ChannelReport>.Succeeded(channelReport);
        }
    }
}
