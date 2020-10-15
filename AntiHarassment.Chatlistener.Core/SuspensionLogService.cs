using AntiHarassment.Chatlistener.Core.Events;
using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using AntiHarassment.Messaging.Events;
using AntiHarassment.Messaging.NServiceBus;
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

        public SuspensionLogService(
            ICompositeChatClient compositeChatClient,
            IDatetimeProvider datetimeProvider,
            ISuspensionRepository suspensionRepository,
            IChatRepository chatRepository,
            IUserRepository userRepository,
            IDataAnalyser dataAnalyser,
            IServiceProvider serviceProvider,
            ISuspensionLogSettings suspensionLogSettings)
        {
            this.compositeChatClient = compositeChatClient;
            this.datetimeProvider = datetimeProvider;
            this.suspensionRepository = suspensionRepository;
            this.chatRepository = chatRepository;
            this.userRepository = userRepository;
            this.dataAnalyser = dataAnalyser;
            this.serviceProvider = serviceProvider;
            this.suspensionLogSettings = suspensionLogSettings;
        }

        public void Start()
        {
            compositeChatClient.OnUserBanned += CompositeChatClient_OnUserBanned;
            compositeChatClient.OnUserTimedOut += CompositeChatClient_OnUserTimedOut;
            compositeChatClient.OnUserUnBanned += CompositeChatClient_OnUserUnBanned;
            compositeChatClient.OnUserUnTimedout += CompositeChatClient_OnUserUnTimedout;
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
            if (!isUnconfirmedSource && userBannedEvent.Source == EventSource.IRC)
                return;

            var suspension = Suspension.CreateBan(userBannedEvent.Username, userBannedEvent.Channel, timeOfSuspension, chatlogForUser, isUnconfirmedSource);
            if (suspension.InvalidSuspension)
            {
                await suspensionRepository.Save(suspension).ConfigureAwait(false);
                return;
            }

            var messageDispatcher = serviceProvider.GetService(typeof(IMessageDispatcher)) as IMessageDispatcher;
            await MarkActiveSuspensionsForUserAsOverwritten(userBannedEvent.Username, $"{suspension.SuspensionType}", timeOfSuspension, messageDispatcher).ConfigureAwait(false);

            var analysedSuspension = await dataAnalyser.AttemptToTagSuspension(suspension).ConfigureAwait(false);
            await SaveAndPublishNewSuspension(analysedSuspension, messageDispatcher).ConfigureAwait(false);
        }

        private async Task CompositeChatClient_OnUserTimedOut(UserTimedoutEvent userTimedoutEvent)
        {
            if (userTimedoutEvent.TimeoutDuration <= suspensionLogSettings.MinimumDurationForTimeouts)
                return;

            var timeOfSuspension = datetimeProvider.UtcNow;
            var chatlogForUser = await chatRepository.GetMessagesFor(userTimedoutEvent.Username, userTimedoutEvent.Channel, suspensionLogSettings.TimeoutChatLogRecordTime, timeOfSuspension).ConfigureAwait(false);

            var userForChannel = await userRepository.GetByTwitchUsername(userTimedoutEvent.Channel).ConfigureAwait(false);
            var isUnconfirmedSource = userForChannel == null;

            // This is probably going to be followed shortly by better event.
            if (!isUnconfirmedSource && userTimedoutEvent.Source == EventSource.IRC)
                return;

            var suspension = Suspension.CreateTimeout(userTimedoutEvent.Username, userTimedoutEvent.Channel, userTimedoutEvent.TimeoutDuration, timeOfSuspension, chatlogForUser, isUnconfirmedSource);
            if (suspension.InvalidSuspension)
            {
                await suspensionRepository.Save(suspension).ConfigureAwait(false);
                return;
            }

            var messageDispatcher = serviceProvider.GetService(typeof(IMessageDispatcher)) as IMessageDispatcher;
            await MarkActiveSuspensionsForUserAsOverwritten(userTimedoutEvent.Username, $"{suspension.SuspensionType} of duration: {suspension.Duration} seconds.", timeOfSuspension, messageDispatcher).ConfigureAwait(false);

            var analysedSuspension = await dataAnalyser.AttemptToTagSuspension(suspension).ConfigureAwait(false);
            await SaveAndPublishNewSuspension(analysedSuspension, messageDispatcher).ConfigureAwait(false);
        }

        private async Task MarkActiveSuspensionsForUserAsOverwritten(string username, string upgradedTo, DateTime timeOfSuspension, IMessageDispatcher messageDispatcher)
        {
            var suspensionsForUser = await suspensionRepository.GetSuspensionsForUser(username).ConfigureAwait(false);

            foreach (var activeSuspension in suspensionsForUser.Where(x => x.IsActive(timeOfSuspension)))
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
