using AntiHarassment.Core.Models;
using AntiHarassment.Messaging.Commands;
using AntiHarassment.Messaging.NServiceBus;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly ISecurity security;
        private readonly IMessageDispatcher messageDispatcher;
        private readonly IDatetimeProvider datetimeProvider;
        private readonly ITwitchApiIntegration twitchApi;

        public UserService(
            IUserRepository userRepository,
            ISecurity security,
            IMessageDispatcher messageDispatcher,
            IDatetimeProvider datetimeProvider,
            ITwitchApiIntegration twitchApi)
        {
            this.userRepository = userRepository;
            this.security = security;
            this.messageDispatcher = messageDispatcher;
            this.datetimeProvider = datetimeProvider;
            this.twitchApi = twitchApi;
        }

        public async Task<IResult<User>> Authenticate(string twitchUsername, string password)
        {
            if (string.IsNullOrEmpty(twitchUsername))
                return Result<User>.Failure("Twitchusername must be provided");

            if (string.IsNullOrEmpty(password))
                return Result<User>.Failure("Password must be provided");

            var user = await userRepository.GetByTwitchUsername(twitchUsername).ConfigureAwait(false);
            if (user == null)
                return Result<User>.Failure("Incorrect username or password");

            if (!security.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return Result<User>.Failure("Incorrect username or password");

            if (!user.EmailConfirmed)
                return Result<User>.Failure("Account registration has not been confirmed");

            return Result<User>.Succeeded(user);
        }

        public async Task<IResult<User>> Authenticate(string accessToken)
        {
            var tokenResult = await twitchApi.GetTwitchUsernameFromToken(accessToken).ConfigureAwait(false);

            var user = await userRepository.GetByTwitchUsername(tokenResult.TwitchUsername).ConfigureAwait(false);
            if (user == null)
                user = User.CreateNewUser(tokenResult.Email, tokenResult.TwitchUsername);
            else
                user.UpdateEmail(tokenResult.Email);

            await userRepository.Save(user).ConfigureAwait(false);

            return Result<User>.Succeeded(user);
        }

        public async Task<IResult> Confirm(string twitchUsername, Guid confirmationToken)
        {
            var user = await userRepository.GetByTwitchUsername(twitchUsername).ConfigureAwait(false);
            if (!user.TryConfirmUserRegistration(confirmationToken))
                return Result.Failure("Invalid Token");

            await userRepository.Save(user).ConfigureAwait(false);

            return Result.Succeeded;
        }

        public async Task<IResult<User>> Create(string email, string twitchUsername, string password, string roleName = null)
        {
            if (!string.IsNullOrEmpty(email) && !EmailIsValid())
                return Result<User>.Failure("Invalid Email");

            // If it exists and hasnt been confirmed we should delete and try again.
            var existingUser = await userRepository.GetByTwitchUsername(twitchUsername).ConfigureAwait(false);
            if (existingUser != null)
            {
                if (existingUser.EmailConfirmed)
                    return Result<User>.Failure("An user with that username already exists.");
                else
                    await userRepository.Delete(existingUser.Id).ConfigureAwait(false);
            }

            if (!PasswordIsValid(password, out var reason))
                return Result<User>.Failure(reason);

            if (!security.TryCreatePasswordHash(password, out var hash, out var salt))
                return Result<User>.Failure("Unable to generate password hash and salt.");

            var user = User.CreateNewUser(email, twitchUsername, hash, salt);
            if (!string.IsNullOrEmpty(roleName))
                user.AddRole(roleName);

            await userRepository.Save(user).ConfigureAwait(false);

            await messageDispatcher.Send(new SendAccountConfirmationCommand() { UserId = user.Id }).ConfigureAwait(false);

            return Result<User>.Succeeded(user);

            bool EmailIsValid()
            {
                try
                {
                    _ = new MailAddress(email);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public async Task<IResult> SendPasswordResetTokenFor(string twitchUsername)
        {
            var user = await userRepository.GetByTwitchUsername(twitchUsername).ConfigureAwait(false);
            if (user == null)
                return Result.Succeeded;

            var now = datetimeProvider.UtcNow;
            if (!user.TryGenerateResetToken(now.AddHours(2), now))
                return Result.Failure("There is an already active reset token.");

            await userRepository.Save(user).ConfigureAwait(false);
            await messageDispatcher.Send(new SendPasswordResetTokenCommand { UserId = user.Id }).ConfigureAwait(false);

            return Result.Succeeded;
        }

        public async Task<IResult> UpdatePasswordFor(string twitchUsername, Guid resetToken, string newPassword)
        {
            var user = await userRepository.GetByTwitchUsername(twitchUsername).ConfigureAwait(false);
            if (user == null)
                return Result.Failure("Was unable to find user");

            if (!user.ResetTokenMatchesAndIsValid(resetToken, datetimeProvider.UtcNow))
                return Result.Failure("Invalid or expired token");

            if (!PasswordIsValid(newPassword, out var reason))
                return Result.Failure(reason);

            if (!security.TryCreatePasswordHash(newPassword, out var hash, out var salt))
                return Result.Failure("Unable to generate password hash and salt.");

            user.UpdatePassword(hash, salt);
            user.InvalidateResetToken();

            await userRepository.Save(user).ConfigureAwait(false);

            return Result.Succeeded;
        }

        private bool PasswordIsValid(string password, out string failureReason)
        {
            if (string.IsNullOrEmpty(password))
            {
                failureReason = "Password cannot be empty";
                return false;
            }

            var hasMinimum8Chars = new Regex(".{8,}");
            if (!hasMinimum8Chars.Match(password).Success)
            {
                failureReason = "Password must contain atleast 8 characters";
                return false;
            }

            var hasNumber = new Regex("[0-9]+");
            if (!hasNumber.Match(password).Success)
            {
                failureReason = "Password must contain atleast one number";
                return false;
            }

            var hasUpperChar = new Regex("[A-Z]+");
            if (!hasUpperChar.Match(password).Success)
            {
                failureReason = "Password must contain atleast one uppercase character";
                return false;
            }

            var hasLowerChar = new Regex("[a-z]+");
            if (!hasLowerChar.Match(password).Success)
            {
                failureReason = "Password must contain atleast one lowercase character";
                return false;
            }

            failureReason = string.Empty;
            return true;
        }
    }
}
