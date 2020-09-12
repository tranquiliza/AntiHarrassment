using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Core;
using AntiHarassment.Messaging.Commands;
using AntiHarassment.Messaging.Events;
using NServiceBus;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Handlers
{
    public class ChannelCommandHandler
        : IHandleMessages<JoinChannelCommand>,
        IHandleMessages<LeaveChannelCommand>,
        IHandleMessages<ChannelChangedSystemModerationEvent>
    {
        private readonly IChatlistenerService chatlistenerService;
        private readonly IUserRepository userRepository;

        public ChannelCommandHandler(IChatlistenerService chatlistenerService, IUserRepository userRepository)
        {
            this.chatlistenerService = chatlistenerService;
            this.userRepository = userRepository;
        }

        public async Task Handle(JoinChannelCommand message, IMessageHandlerContext context)
        {
            var user = await userRepository.GetById(message.RequestedByUserId).ConfigureAwait(false);
            var applicationContext = ApplicatonContext.CreateFromUser(user);

            await chatlistenerService.ListenTo(message.ChannelName, applicationContext).ConfigureAwait(false);

            await context.Publish(new JoinedChannelEvent(message.ChannelName)).ConfigureAwait(false);
        }

        public async Task Handle(LeaveChannelCommand message, IMessageHandlerContext context)
        {
            var user = await userRepository.GetById(message.RequestedByUserId).ConfigureAwait(false);
            var applicationContext = ApplicatonContext.CreateFromUser(user);

            await chatlistenerService.UnlistenTo(message.ChannelName, applicationContext).ConfigureAwait(false);

            await context.Publish(new LeftChannelEvent(message.ChannelName)).ConfigureAwait(false);
        }

        public async Task Handle(ChannelChangedSystemModerationEvent message, IMessageHandlerContext context)
        {
            var user = await userRepository.GetById(message.RequestedByUserId).ConfigureAwait(false);
            var applicationContext = ApplicatonContext.CreateFromUser(user);

            // TODO This works for now. Should possibly be more specific that use is enabling / disabling this feature.
            if (message.SystemIsModerator)
            {
                await chatlistenerService.JoinPubSub(message.ChannelName, applicationContext).ConfigureAwait(false);
                await context.Publish(new AutoModListenerEnabledForChannelEvent(message.ChannelName)).ConfigureAwait(false);
            }
            else
            {
                await chatlistenerService.LeavePubSub(message.ChannelName, applicationContext).ConfigureAwait(false);
                await context.Publish(new AutoModListenerDisabledForChannelEvent(message.ChannelName)).ConfigureAwait(false);
            }
        }
    }
}
