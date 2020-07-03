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

        Task Initialize();
        Task CreateAccount(RegisterUserModel model);
        Task<bool> TryLogin(AuthenticateModel model);
        Task<bool> TryLogout();
    }
}
