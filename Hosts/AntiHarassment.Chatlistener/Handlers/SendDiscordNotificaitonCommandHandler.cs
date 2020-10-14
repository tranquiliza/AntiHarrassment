using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Messaging.Commands;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Handlers
{
    public class SendDiscordNotificaitonCommandHandler : IHandleMessages<SendDiscordNotificaitonCommand>
    {
        private readonly IDiscordNotificationService discordNotificationService;
        private readonly ILogger<SendAccountConfirmationCommandHandler> logger;

        public SendDiscordNotificaitonCommandHandler(IDiscordNotificationService discordNotificationService, ILogger<SendAccountConfirmationCommandHandler> logger)
        {
            this.discordNotificationService = discordNotificationService;
            this.logger = logger;
        }

        public Task Handle(SendDiscordNotificaitonCommand message, IMessageHandlerContext context)
        {
            logger.LogInformation($"Received command to send discord Notification for {message.Username} in {message.ChannelOfOrigin}");

            return discordNotificationService.SendNotification(message.Username, message.ChannelOfOrigin, message.RuleId);
        }
    }
}
