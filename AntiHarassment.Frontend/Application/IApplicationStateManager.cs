using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Application
{
    public interface IApplicationStateManager
    {
        Task<string> GetJwtToken();
        Task SetJwtToken(string token);
        Task<string> GetTwitchAccessToken();
        Task SetTwitchAccessToken(string accessToken);
        Task<List<string>> GetTwitchScopes();
        Task SetTwitchScopes(List<string> scopes);
    }
}
