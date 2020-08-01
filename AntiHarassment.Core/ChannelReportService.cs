using AntiHarassment.Core.Models;
using AntiHarassment.Core.Security;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public class ChannelReportService : IChannelReportService
    {
        private readonly IUserReportService userReportService;
        private readonly IChannelRepository channelRepository;
        private readonly ISuspensionRepository suspensionRepository;
        private readonly ILogger<ChannelReportService> logger;

        public ChannelReportService(
            IUserReportService userReportService,
            IChannelRepository channelRepository,
            ISuspensionRepository suspensionRepository,
            ILogger<ChannelReportService> logger)
        {
            this.userReportService = userReportService;
            this.channelRepository = channelRepository;
            this.suspensionRepository = suspensionRepository;
            this.logger = logger;
        }

        public async Task<IResult<ChannelReport>> GenerateReportForChannel(string channelName, IApplicationContext context)
        {
            var channel = await channelRepository.GetChannel(channelName).ConfigureAwait(false);
            if (!context.HaveAccessTo(channel))
                return Result<ChannelReport>.Unauthorized();

            var allUsersSuspendedInChannel = await suspensionRepository.GetSuspendedUsersForChannel(channelName).ConfigureAwait(false);
            if (allUsersSuspendedInChannel.Count == 0)
                return Result<ChannelReport>.NoContentFound();

            var userReports = new List<UserReport>();

            foreach (var username in allUsersSuspendedInChannel)
            {
                var userReport = await userReportService.GetUserReportFor(username).ConfigureAwait(false);
                if (userReport.State != ResultState.Success)
                {
                    logger.LogWarning("Was unable to generage a user report for {arg}", username);
                    return Result<ChannelReport>.Failure(userReport.FailureReason);
                }

                userReports.Add(userReport.Data);
            }

            var channelReport = new ChannelReport(channelName, userReports);
            return Result<ChannelReport>.Succeeded(channelReport);
        }
    }
}
