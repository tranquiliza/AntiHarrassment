using System;

namespace AntiHarassment.Contract
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public ulong DiscordUserId { get; set; }
        public bool HasDiscordNotificationsEnabled { get; set; }
    }
}
