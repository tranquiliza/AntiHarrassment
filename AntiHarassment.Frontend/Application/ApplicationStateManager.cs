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

        public async Task<string> GetJwtToken()
        {
            return await _applicationState.GetItem<string>(_jwtTokenKey).ConfigureAwait(false);
        }

        public async Task SetJwtToken(string token)
        {
            await _applicationState.SetItem(_jwtTokenKey, token).ConfigureAwait(false);
        }
    }
}
