using AntiHarassment.Core.Models;
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

        public UserService(IUserRepository userRepository, ISecurity security)
        {
            this.userRepository = userRepository;
            this.security = security;
        }

        public async Task<IResult<User>> Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email))
                return Result<User>.Failure("Email must be provided");

            if (string.IsNullOrEmpty(password))
                return Result<User>.Failure("Password must be provided");

            var user = await userRepository.GetByEmail(email).ConfigureAwait(false);
            if (user == null)
                return Result<User>.Failure("Incorrect email or username");

            if (!security.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return Result<User>.Failure("Incorrect email or username");

            return Result<User>.Succeeded(user);
        }

        public async Task<IResult<User>> Create(string email, string password, string roleName = null)
        {
            if (!EmailIsValid())
                return Result<User>.Failure("Invalid Email");

            if (await userRepository.GetByEmail(email).ConfigureAwait(false) != null)
                return Result<User>.Failure("A user with that username already exists.");

            if (!PasswordIsValid(out var reason))
                return Result<User>.Failure(reason);

            if (!security.TryCreatePasswordHash(password, out var hash, out var salt))
                return Result<User>.Failure("Unable to generate password hash and salt.");

            var user = User.CreateNewUser(email, hash, salt);
            if (!string.IsNullOrEmpty(roleName))
                user.AddRole(roleName);

            await userRepository.Save(user).ConfigureAwait(false);

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

            bool PasswordIsValid(out string failureReason)
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
}
