using AntiHarassment.Contract;
using AntiHarassment.Frontend.Infrastructure;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Application
{
    public class UserService : IUserService
    {
        public class UserInformation
        {
            public Guid Id { get; private set; }
            public List<string> Roles { get; private set; }
            public DateTime TokenExpires { get; private set; }
            public string TwitchUsername { get; private set; }

            public UserInformation(Guid id, List<string> roles, DateTime tokenExpires, string twitchUsername)
            {
                Id = id;
                Roles = roles;
                TokenExpires = tokenExpires;
                TwitchUsername = twitchUsername;
            }
        }

        private readonly IApiGateway apiGateway;
        private readonly IApplicationStateManager applicationStateManager;
        public UserInformation User { get; private set; }

        private void NotifyStateChanged() => OnChange?.Invoke();
        public event Action OnChange;

        public bool IsUserLoggedIn => User != null;
        public bool IsUserAdmin => User?.Roles.Any(role => string.Equals("ADMIN", role, StringComparison.Ordinal)) == true;
        public string CurrentUserTwitchUsername => User?.TwitchUsername;

        public UserService(IApiGateway apiGateway, IApplicationStateManager applicationStateManager)
        {
            this.apiGateway = apiGateway;
            this.applicationStateManager = applicationStateManager;
        }

        public async Task Initialize()
        {
            var currentToken = await applicationStateManager.GetJwtToken().ConfigureAwait(false);
            if (!string.IsNullOrEmpty(currentToken))
            {
                User = await CreateUserFromJwtToken(currentToken).ConfigureAwait(false);
                NotifyStateChanged();
            }
        }

        private async Task<UserInformation> CreateUserFromJwtToken(string jwtToken)
        {
            if (string.IsNullOrEmpty(jwtToken))
                return null;

            var jwt = new JwtSecurityToken(jwtToken);
            var uniqueName = jwt.Claims.FirstOrDefault(x => x.Type == "unique_name");
            var twitchUsername = jwt.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.TwitchUsername);
            if (!Guid.TryParse(uniqueName?.Value, out var userId))
                return null;

            var expires = jwt.ValidTo;
            if (expires <= DateTime.UtcNow)
            {
                await TryLogout().ConfigureAwait(false);
                return null;
            }

            var roles = jwt.Claims.Where(x => x.Type == "role").Select(x => x.Value).ToList();

            return new UserInformation(userId, roles, expires, twitchUsername?.Value);
        }

        public string LoginError { get; private set; }
        public async Task<bool> TryLogin(AuthenticateModel model)
        {
            try
            {
                var response = await apiGateway.Post<UserAuthenticatedModel, AuthenticateModel>(model, "Users", "Authenticate").ConfigureAwait(false);
                if (response == null)
                    return false;

                await applicationStateManager.SetJwtToken(response.Token).ConfigureAwait(false);
                User = await CreateUserFromJwtToken(response.Token).ConfigureAwait(false);
                NotifyStateChanged();
                return true;
            }
            catch (Exception ex)
            {
                LoginError = ex.Message;
                return false;
            }
        }

        public async Task<bool> TryLogout()
        {
            User = null;
            await applicationStateManager.SetJwtToken(string.Empty).ConfigureAwait(false);

            NotifyStateChanged();
            return true;
        }

        public string CreateAccountError { get; private set; }

        public async Task<bool> CreateAccount(RegisterUserModel model)
        {
            try
            {
                await apiGateway.Post(model, "Users").ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                CreateAccountError = ex.Message;
                return false;
            }
        }
    }
}
