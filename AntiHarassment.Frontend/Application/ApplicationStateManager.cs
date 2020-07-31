using AntiHarassment.Frontend.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Application
{
    public class ApplicationStateManager : IApplicationStateManager
    {
        private readonly IApplicationState _applicationState;

        public ApplicationStateManager(IApplicationState applicationState)
        {
            _applicationState = applicationState;
        }

        private const string _jwtTokenKey = "JwtToken";
        private const string _twitchAccessToken = "TwitchAccessToken";
        private const string _twitchScopes = "TwitchScopes";

        public async Task<string> GetJwtToken()
        {
            return await _applicationState.GetItem<string>(_jwtTokenKey).ConfigureAwait(false);
        }

        public async Task SetJwtToken(string token)
        {
            await _applicationState.SetItem(_jwtTokenKey, token).ConfigureAwait(false);
        }

        public async Task<string> GetTwitchAccessToken()
        {
            return await _applicationState.GetItem<string>(_twitchAccessToken).ConfigureAwait(false);
        }

        public async Task SetTwitchAccessToken(string accessToken)
        {
            await _applicationState.SetItem(_twitchAccessToken, accessToken).ConfigureAwait(false);
        }

        public async Task<List<string>> GetTwitchScopes()
        {
            return await _applicationState.GetItem<List<string>>(_twitchScopes).ConfigureAwait(false);
        }

        public async Task SetTwitchScopes(List<string> scopes)
        {
            await _applicationState.SetItem(_twitchScopes, scopes).ConfigureAwait(false);
        }
    }
}
