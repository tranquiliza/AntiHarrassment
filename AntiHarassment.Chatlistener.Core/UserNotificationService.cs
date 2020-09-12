using AntiHarassment.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public class UserNotificationService : IUserNotificationService
    {
        private readonly IChatClient chatClient;
        private readonly IUserRepository userRepository;
        private readonly ILogger<UserNotificationService> logger;

        public UserNotificationService(IChatClient chatClient, IUserRepository userRepository, ILogger<UserNotificationService> logger)
        {
            this.chatClient = chatClient;
            this.userRepository = userRepository;
            this.logger = logger;
        }

        public async Task SendConfirmationTokenToUser(Guid userId)
        {
            var user = await userRepository.GetById(userId).ConfigureAwait(false);
            if (user == null)
                return;

            var message = $"This is your token: {user.EmailConfirmationToken}";
            await chatClient.SendWhisper(user.TwitchUsername, message).ConfigureAwait(false);
            logger.LogInformation("Sent confirm token whisper to {username}", user.TwitchUsername);
        }

        public async Task SendPassworkResetTokenToUser(Guid userId)
        {
            var user = await userRepository.GetById(userId).ConfigureAwait(false);
            if (user == null)
                return;

            var message = $"This is your password reset token: {user.ResetToken}";
            await chatClient.SendWhisper(user.TwitchUsername, message).ConfigureAwait(false);
            logger.LogInformation("Sent password reset token whisper to {username}", user.TwitchUsername);
        }
    }
}
