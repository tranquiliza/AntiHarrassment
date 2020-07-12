using AntiHarassment.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public class UserNotificationService : IUserNotificationService
    {
        private readonly IChatClient chatClient;
        private readonly IUserRepository userRepository;

        public UserNotificationService(IChatClient chatClient, IUserRepository userRepository)
        {
            this.chatClient = chatClient;
            this.userRepository = userRepository;
        }

        public async Task SendConfirmationTokenToUser(Guid userId)
        {
            var user = await userRepository.GetById(userId).ConfigureAwait(false);
            if (user == null)
                return;

            var message = $"This is your token: {user.EmailConfirmationToken}";
            await chatClient.SendWhisper(user.TwitchUsername, message).ConfigureAwait(false);
        }

        public async Task SendPassworkResetTokenToUser(Guid userId)
        {
            var user = await userRepository.GetById(userId).ConfigureAwait(false);
            if (user == null)
                return;

            var message = $"This is your password reset token: {user.ResetToken}";
            await chatClient.SendWhisper(user.TwitchUsername, message).ConfigureAwait(false);
        }
    }
}
