using AntiHarassment.Chatlistener.Core.Events;
using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using AntiHarassment.Messaging.Events;
using AntiHarassment.Messaging.NServiceBus;
using Microsoft.Extensions.Logging;
using MoreLinq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public class SuspensionLogService : ISuspensionLogService
    {
        private readonly ICompositeChatClient compositeChatClient;
        private readonly IDatetimeProvider datetimeProvider;
        private readonly ISuspensionRepository suspensionRepository;
        private readonly IChatRepository chatRepository;
        private readonly IUserRepository userRepository;
        private readonly IDataAnalyser dataAnalyser;
        private readonly IServiceProvider serviceProvider;
        private readonly ISuspensionLogSettings suspensionLogSettings;
        private readonly ILogger<SuspensionLogService> logger;

        private const string SYSTEM_USERNAME = "AntiHarassment";

        public SuspensionLogService(
            ICompositeChatClient compositeChatClient,
            IDatetimeProvider datetimeProvider,
            ISuspensionRepository suspensionRepository,
            IChatRepository chatRepository,
            IUserRepository userRepository,
            IDataAnalyser dataAnalyser,
            IServiceProvider serviceProvider,
            ISuspensionLogSettings suspensionLogSettings,
            ILogger<SuspensionLogService> logger)
        {
            this.compositeChatClient = compositeChatClient;
            this.datetimeProvider = datetimeProvider;
            this.suspensionRepository = suspensionRepository;
            this.chatRepository = chatRepository;
            this.userRepository = userRepository;
            this.dataAnalyser = dataAnalyser;
            this.serviceProvider = serviceProvider;
            this.suspensionLogSettings = suspensionLogSettings;
            this.logger = logger;
        }

        public void Start()
        {
            logger.LogInformation("Starting Suspension Logging");
            compositeChatClient.OnUserBanned += CompositeChatClient_OnUserBanned;
            compositeChatClient.OnUserTimedOut += CompositeChatClient_OnUserTimedOut;
            compositeChatClient.OnUserUnBanned += CompositeChatClient_OnUserUnBanned;
            compositeChatClient.OnUserUnTimedout += CompositeChatClient_OnUserUnTimedout;
            logger.LogInformation("Suspension Logging Initiated");
        }

        private async Task CompositeChatClient_OnUserUnTimedout(UserUntimedoutEvent userUntimedoutEvent)
        {
            var messageDispatcher = serviceProvider.GetService(typeof(IMessageDispatcher)) as IMessageDispatcher;
            var timeOfSuspension = datetimeProvider.UtcNow;

            await MarkActiveSuspensionsForUserAsUndone(userUntimedoutEvent.Username, userUntimedoutEvent.UntimedoutBy, timeOfSuspension, messageDispatcher).ConfigureAwait(false);
        }

        private async Task CompositeChatClient_OnUserUnBanned(UserUnbannedEvent userUnbannedEvent)
        {
            var messageDispatcher = serviceProvider.GetService(typeof(IMessageDispatcher)) as IMessageDispatcher;
            var timeOfSuspension = datetimeProvider.UtcNow;

            await MarkActiveSuspensionsForUserAsUndone(userUnbannedEvent.Username, userUnbannedEvent.UnbannedBy, timeOfSuspension, messageDispatcher).ConfigureAwait(false);
        }

        private async Task MarkActiveSuspensionsForUserAsUndone(string username, string undoneBy, DateTime timeOfSuspension, IMessageDispatcher messageDispatcher)
        {
            var suspensionsForUser = await suspensionRepository.GetSuspensionsForUser(username).ConfigureAwait(false);

            foreach (var activeSuspension in suspensionsForUser.Where(x => x.IsActive(timeOfSuspension)))
            {
                activeSuspension.MarkSuspensionAsUndone(undoneBy);
                await suspensionRepository.Save(activeSuspension).ConfigureAwait(false);
                await messageDispatcher.Publish(new SuspensionUpdatedEvent
                {
                    ChannelOfOrigin = activeSuspension.ChannelOfOrigin,
                    SuspensionId = activeSuspension.SuspensionId
                }).ConfigureAwait(false);
            }
        }

        private async Task CompositeChatClient_OnUserBanned(UserBannedEvent userBannedEvent)
        {
            var timeOfSuspension = datetimeProvider.UtcNow;
            var chatlogForUser = await chatRepository.GetMessagesFor(userBannedEvent.Username, userBannedEvent.Channel, suspensionLogSettings.BanChatLogRecordTime, timeOfSuspension).ConfigureAwait(false);

            var userForChannel = await userRepository.GetByTwitchUsername(userBannedEvent.Channel).ConfigureAwait(false);
            var isUnconfirmedSource = userForChannel == null;

            // This is probably going to be followed shortly by better event.
            //if (!isUnconfirmedSource && userBannedEvent.Source == EventSource.IRC)
            //{
            //    logger.LogInformation("Received a ban from {channel}, on person {person}, but it was from channel with mod status, so we're skipping it", userBannedEvent.Channel, userBannedEvent.Username);
            //    return;
            //}

            var messageDispatcher = serviceProvider.GetService(typeof(IMessageDispatcher)) as IMessageDispatcher;
            if (isSystemIssuedBan())
            {
                var systemSuspension = await CreateSystemSuspensionWithEvidence(userBannedEvent.Username, userBannedEvent.Channel, timeOfSuspension, userBannedEvent.BanReason).ConfigureAwait(false);
                await MarkActiveSuspensionsForUserAsOverwritten(userBannedEvent.Username, userBannedEvent.Channel, "SYSTEM BAN", timeOfSuspension, messageDispatcher).ConfigureAwait(false);

                await SaveAndPublishNewSuspension(systemSuspension, messageDispatcher).ConfigureAwait(false);
                return;
            }

            var suspension = Suspension.CreateBan(userBannedEvent.Username, userBannedEvent.Channel, timeOfSuspension, chatlogForUser, isUnconfirmedSource);
            if (suspension.InvalidSuspension)
            {
                await suspensionRepository.Save(suspension).ConfigureAwait(false);
                return;
            }

            await MarkActiveSuspensionsForUserAsOverwritten(userBannedEvent.Username, userBannedEvent.Channel, $"{suspension.SuspensionType}", timeOfSuspension, messageDispatcher).ConfigureAwait(false);

            var analysedSuspension = await dataAnalyser.AttemptToTagSuspension(suspension).ConfigureAwait(false);
            await SaveAndPublishNewSuspension(analysedSuspension, messageDispatcher).ConfigureAwait(false);

            bool isSystemIssuedBan()
                => !isUnconfirmedSource && (string.Equals(userBannedEvent.BannedBy, SYSTEM_USERNAME, StringComparison.OrdinalIgnoreCase) || userBannedEvent.BanReason.StartsWith("[AHS]"));
        }

        private async Task CompositeChatClient_OnUserTimedOut(UserTimedoutEvent userTimedoutEvent)
        {
            if (userTimedoutEvent.TimeoutDuration <= suspensionLogSettings.MinimumDurationForTimeouts)
                return;

            var timeOfSuspension = datetimeProvider.UtcNow;
            var chatlogForUser = await chatRepository.GetMessagesFor(userTimedoutEvent.Username, userTimedoutEvent.Channel, suspensionLogSettings.TimeoutChatLogRecordTime, timeOfSuspension).ConfigureAwait(false);

            var userForChannel = await userRepository.GetByTwitchUsername(userTimedoutEvent.Channel).ConfigureAwait(false);
            var isUnconfirmedSource = userForChannel == null;

            //// This is probably going to be followed shortly by better event.
            //if (!isUnconfirmedSource && userTimedoutEvent.Source == EventSource.IRC)
            //    return;

            var suspension = Suspension.CreateTimeout(userTimedoutEvent.Username, userTimedoutEvent.Channel, userTimedoutEvent.TimeoutDuration, timeOfSuspension, chatlogForUser, isUnconfirmedSource);
            if (suspension.InvalidSuspension)
            {
                await suspensionRepository.Save(suspension).ConfigureAwait(false);
                return;
            }

            var messageDispatcher = serviceProvider.GetService(typeof(IMessageDispatcher)) as IMessageDispatcher;
            await MarkActiveSuspensionsForUserAsOverwritten(userTimedoutEvent.Username, userTimedoutEvent.Channel, $"{suspension.SuspensionType} of duration: {suspension.Duration} seconds.", timeOfSuspension, messageDispatcher).ConfigureAwait(false);

            var analysedSuspension = await dataAnalyser.AttemptToTagSuspension(suspension).ConfigureAwait(false);
            await SaveAndPublishNewSuspension(analysedSuspension, messageDispatcher).ConfigureAwait(false);
        }

        private async Task MarkActiveSuspensionsForUserAsOverwritten(string username, string channelOfOrigin, string upgradedTo, DateTime timeOfSuspension, IMessageDispatcher messageDispatcher)
        {
            var suspensionsForUser = await suspensionRepository.GetSuspensionsForUser(username).ConfigureAwait(false);

            foreach (var activeSuspension in suspensionsForUser.Where(x => x.IsActive(timeOfSuspension) && string.Equals(channelOfOrigin, x.ChannelOfOrigin, StringComparison.OrdinalIgnoreCase)))
            {
                activeSuspension.MarkSuspensionAsOverwritten(upgradedTo);
                await suspensionRepository.Save(activeSuspension).ConfigureAwait(false);
                await messageDispatcher.Publish(new SuspensionUpdatedEvent
                {
                    ChannelOfOrigin = activeSuspension.ChannelOfOrigin,
                    SuspensionId = activeSuspension.SuspensionId
                }).ConfigureAwait(false);
            }
        }

        // TODO Consider if its worthwhile adding chatlogs? (This could be alot of chatlogs)
        private async Task<Suspension> CreateSystemSuspensionWithEvidence(string username, string channel, DateTime timeOfSuspension, string banReason)
        {
            var suspensionsForUser = await suspensionRepository.GetSuspensionsForUser(username).ConfigureAwait(false);
            var validSuspensions = suspensionsForUser.Where(x => x.SuspensionSource != SuspensionSource.System && !x.InvalidSuspension);

            var tags = validSuspensions.SelectMany(x => x.Tags).DistinctBy(x => x.TagId);

            var systemSuspension = Suspension.CreateSystemBan(username, channel, timeOfSuspension, banReason);

            var systemContext = new SystemAppContext();

            foreach (var tag in tags)
                systemSuspension.TryAddTag(tag, systemContext, timeOfSuspension);

            return systemSuspension;
        }

        private async Task SaveAndPublishNewSuspension(Suspension suspension, IMessageDispatcher messageDispatcher)
        {
            await suspensionRepository.Save(suspension).ConfigureAwait(false);
            await messageDispatcher.Publish(new NewSuspensionEvent
            {
                SuspensionId = suspension.SuspensionId,
                ChannelOfOrigin = suspension.ChannelOfOrigin
            }).ConfigureAwait(false);
        }

        public void Dispose()
        {
            compositeChatClient.OnUserBanned -= CompositeChatClient_OnUserBanned;
            compositeChatClient.OnUserTimedOut -= CompositeChatClient_OnUserTimedOut;
            compositeChatClient.OnUserUnBanned -= CompositeChatClient_OnUserUnBanned;
            compositeChatClient.OnUserUnTimedout -= CompositeChatClient_OnUserUnTimedout;
        }
    }
}
