using AntiHarassment.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Application
{
    public interface IUserService
    {
        event Action OnChange;

        bool IsUserLoggedIn { get; }
        bool IsUserAdmin { get; }
        string CurrentUserTwitchUsername { get; }

        string CreateAccountError { get; }
        string LoginError { get; }
        string ConfirmAccountTokenError { get; }
        string RequestPasswordResetError { get; }
        string ResetPasswordError { get; }

        Task Initialize();
        Task<bool> CreateAccount(RegisterUserModel model);
        Task<bool> TryLogin(AuthenticateModel model);
        Task<bool> ConfirmToken(string confirmationUsername, string confirmationToken);
        Task<bool> TryLogout();
        Task<bool> RequestPasswordReset(RequestResetTokenModel model);
        Task<bool> ResetPassword(ResetPasswordModel model);
        Task<bool> SetTokensAndLoginWithTwitch(string accessToken);
    }
}
