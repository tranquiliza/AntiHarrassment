using AntiHarassment.Core.Models;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public interface ITwitchApiIntegration
    {
        Task<ITwitchAccessTokenResult> GetTwitchUsernameFromToken(string accessToken);
    }
}
