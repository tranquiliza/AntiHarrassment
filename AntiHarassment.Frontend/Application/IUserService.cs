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

        Task Initialize();
        Task<bool> CreateAccount(RegisterUserModel model);
        Task<bool> TryLogin(AuthenticateModel model);
        Task<bool> TryLogout();
    }
}
