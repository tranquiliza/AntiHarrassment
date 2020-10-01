using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

// Possibly need a transition property in the future if changing property names.
// https://stackoverflow.com/questions/43714050/multiple-jsonproperty-name-assigned-to-single-property

namespace AntiHarassment.Core.Models
{
    public sealed class User
    {
        [JsonProperty]
        public Guid Id { get; private set; }

        [JsonProperty]
        public string TwitchUsername { get; private set; }

        [JsonProperty]
        public string Email { get; private set; }

        [JsonProperty]
        public bool IsLocked { get; set; }

        [JsonProperty]
        public byte[] PasswordHash { get; private set; }

        [JsonProperty]
        public byte[] PasswordSalt { get; private set; }

        [JsonProperty]
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Backing field for roles should be lowercase")]
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

        private User() { }

        private User(string email, string twitchUsername, byte[] passwordHash, byte[] passwordSalt)
        {
            Id = Guid.NewGuid();
            Email = email;
            TwitchUsername = twitchUsername;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            EmailConfirmationToken = Guid.NewGuid();
        }

        private User(string email, string twitchUsername)
        {
            Id = Guid.NewGuid();
            Email = email;
            TwitchUsername = twitchUsername;
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

        internal bool TryGenerateResetToken(DateTime tokenExpirationTime, DateTime currentTime)
        {
            // If we already have a token, there is no reason to send a new one. Use the current. (Prevents flooding too)
            if (ResetTokenExpiration != default && ResetTokenExpiration > currentTime) // If we have no token, we need.
                return false;

            ResetToken = Guid.NewGuid();
            ResetTokenExpiration = tokenExpirationTime;

            return true;
        }

        internal bool ResetTokenMatchesAndIsValid(Guid resetToken, DateTime now) => resetToken == ResetToken && now < ResetTokenExpiration;

        internal void InvalidateResetToken()
        {
            ResetToken = Guid.Empty;
            ResetTokenExpiration = default;
        }

        internal bool TryConfirmUserRegistration(Guid confirmationToken)
        {
            if (confirmationToken != EmailConfirmationToken)
                return false;

            EmailConfirmed = true;
            EmailConfirmationToken = default;
            return true;
        }

        internal void UpdateEmail(string newEmail)
        {
            Email = newEmail;
        }

        internal void LockUser()
        {
            IsLocked = true;
        }

        internal void UnlockUser()
        {
            IsLocked = false;
        }

        internal static User CreateNewUser(string email, string twitchUsername, byte[] passwordHash, byte[] passwordSalt) => new User(email, twitchUsername, passwordHash, passwordSalt);
        internal static User CreateNewUser(string email, string twitchUsername) => new User(email, twitchUsername) { EmailConfirmed = true };
        public static User CreateSystemUser() => new User() { TwitchUsername = "AntiHarassment", Id = Guid.Parse("ead96d55-4e9a-4b8a-9476-3ef54274c0aa") };
    }
}
