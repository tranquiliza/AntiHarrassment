using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AntiHarassment.Core.Models
{
    public class User
    {
        [JsonProperty]
        public Guid Id { get; private set; }

        [JsonProperty]
        public string TwitchUsername { get; private set; }

        [JsonProperty]
        public string Email { get; private set; }

        [JsonProperty]
        public byte[] PasswordHash { get; private set; }

        [JsonProperty]
        public byte[] PasswordSalt { get; private set; }

        [JsonProperty]
        private List<string> roles { get; set; } = new List<string>();

        [JsonProperty]
        public bool EmailConfirmed { get; private set; }

        [JsonProperty]
        public Guid EmailConfirmationToken { get; private set; }

        [JsonProperty]
        public Guid ResetToken { get; private set; }

        [JsonProperty]
        public DateTime ResetTokenExpiration { get; private set; }

        [JsonIgnore]
        public IReadOnlyList<string> Roles => roles.AsReadOnly();

        [Obsolete("Serialization Only", true)]
        public User() { }

        private User(string email, string twitchUsername, byte[] passwordHash, byte[] passwordSalt)
        {
            Id = Guid.NewGuid();
            Email = email;
            TwitchUsername = twitchUsername;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            EmailConfirmationToken = Guid.NewGuid();
        }

        internal void AddRole(string role)
        {
            if (!roles.Contains(role))
                roles.Add(role);
        }

        internal bool HasRole(string role)
        {
            return roles.Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));
        }

        internal void UpdatePassword(byte[] passwordHash, byte[] passwordSalt)
        {
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
        }

        internal Guid GenerateResetToken(DateTime tokenExpirationTime)
        {
            ResetToken = Guid.NewGuid();
            ResetTokenExpiration = tokenExpirationTime;

            return ResetToken;
        }

        internal bool ResetTokenMatchesAndIsValid(Guid resetToken, DateTime now) => resetToken == ResetToken && now < ResetTokenExpiration;

        internal void InvalidateResetToken()
        {
            ResetToken = Guid.Empty;
            ResetTokenExpiration = default;
        }

        internal bool TryConfirmEmail(Guid confirmationToken)
        {
            if (confirmationToken != EmailConfirmationToken)
                return false;

            EmailConfirmed = true;
            return true;
        }

        internal static User CreateNewUser(string email, string twitchUsername, byte[] passwordHash, byte[] passwordSalt) => new User(email, twitchUsername, passwordHash, passwordSalt);
    }
}
