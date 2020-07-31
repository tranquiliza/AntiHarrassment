using AntiHarassment.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public interface ITwitchApiIntegration
    {
        Task<ITwitchAccessTokenResult> GetTwitchUsernameFromToken(string accessToken);
    }
}
