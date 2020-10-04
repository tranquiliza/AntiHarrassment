using AntiHarassment.Contract;
using AntiHarassment.Core.Models;

namespace AntiHarassment.WebApi.Mappers
{
    public static class UserMapper
    {
        public static UserModel Map(this User user)
        {
            if (user == null)
                return null;

            return new UserModel
            {
                Email = user.Email,
                Username = user.TwitchUsername,
                Id = user.Id,
                DiscordUserId = user.DiscordUserId,
                HasDiscordNotificationsEnabled = user.DiscordEnabled
            };
        }

        public static UserAuthenticatedModel Map(this User user, string token)
        {
            if (user == null)
                return null;

            return new UserAuthenticatedModel
            {
                Id = user.Id,
                Username = user.TwitchUsername,
                Email = user.Email,
                Token = token,
                DiscordUserId = user.DiscordUserId,
                HasEnabledDiscordNotifications = user.DiscordEnabled
            };
        }
    }
}
