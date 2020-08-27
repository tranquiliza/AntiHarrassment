using AntiHarassment.Contract;
using AntiHarassment.Frontend.Infrastructure;
using AntiHarassment.SignalR.Contract;
using AntiHarassment.SignalR.Contract.EventArgs;
using Microsoft.JSInterop;
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
        private readonly ChannelsHubSignalRClient channelsHubSignalRClient;
        private readonly SuspensionsHubSignalRClient suspensionsHubSignalRClient;
        private readonly NotificationHubSignalRClient notificationHubSignalRClient;
        private readonly IJSRuntime jSRuntime;

        public UserInformation User { get; private set; }

        private void NotifyStateChanged() => OnChange?.Invoke();
        public event Action OnChange;

        // TODO FIX THE SIGNALR Clients so they disconnect once the login expires.
        public bool IsUserLoggedIn
        {
            get
            {
                if (User != null && DateTime.UtcNow < User.TokenExpires)
                    return true;

                channelsHubSignalRClient.DisposeAsync();
                suspensionsHubSignalRClient.DisposeAsync();
                notificationHubSignalRClient.DisposeAsync();

                return false;
            }
        }

        public bool IsUserAdmin => User?.Roles.Any(role => string.Equals("ADMIN", role, StringComparison.Ordinal)) == true;
        public string CurrentUserTwitchUsername => User?.TwitchUsername;

        public UserService(
            IApiGateway apiGateway,
            IApplicationStateManager applicationStateManager,
            ChannelsHubSignalRClient channelsHubSignalRClient,
            SuspensionsHubSignalRClient suspensionsHubSignalRClient,
            NotificationHubSignalRClient notificationHubSignalRClient,
            IJSRuntime jSRuntime)
        {
            this.apiGateway = apiGateway;
            this.applicationStateManager = applicationStateManager;
            this.channelsHubSignalRClient = channelsHubSignalRClient;
            this.suspensionsHubSignalRClient = suspensionsHubSignalRClient;
            this.notificationHubSignalRClient = notificationHubSignalRClient;
            this.jSRuntime = jSRuntime;

            notificationHubSignalRClient.NotificationReceived += NotificationHubSignalRClient_NotificationReceived;
        }

        private void NotificationHubSignalRClient_NotificationReceived(object _, NotificationEventArgs e)
        {
            var textToDisplay = $"{e.Username} joined {e.ChannelOfOrigin} warning from {e.RuleName}";
            jSRuntime.InvokeVoidAsync("SendToast", textToDisplay);
        }

        public async Task Initialize()
        {
            var currentToken = await applicationStateManager.GetJwtToken().ConfigureAwait(false);
            if (!string.IsNullOrEmpty(currentToken))
            {
                User = await CreateUserFromJwtToken(currentToken).ConfigureAwait(false);
                if (User != null)
                    await notificationHubSignalRClient.StartAsync(User.TwitchUsername).ConfigureAwait(false);

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

                await FinalizeLogin(response).ConfigureAwait(false);
                await notificationHubSignalRClient.StartAsync(User.TwitchUsername).ConfigureAwait(false);
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

        public string ConfirmAccountTokenError { get; private set; }
        public async Task<bool> ConfirmToken(string confirmationUsername, string confirmationToken)
        {
            try
            {
                if (!Guid.TryParse(confirmationToken, out var guidToken))
                {
                    ConfirmAccountTokenError = "Invalid Token";
                    return false;
                }

                var model = new ConfirmUserModel { ConfirmationToken = guidToken };
                await apiGateway.Post(model, "users", "confirm", new string[] { confirmationUsername }).ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                ConfirmAccountTokenError = ex.Message;
                return false;
            }
        }

        public string RequestPasswordResetError { get; private set; }
        public async Task<bool> RequestPasswordReset(RequestResetTokenModel model)
        {
            try
            {
                await apiGateway.Post(model, "users", "requestResetPasswordToken").ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                RequestPasswordResetError = ex.Message;
                return false;
            }
        }

        public string ResetPasswordError { get; private set; }
        public async Task<bool> ResetPassword(ResetPasswordModel model)
        {
            try
            {
                await apiGateway.Post(model, "users", "UpdatePassword").ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                ResetPasswordError = ex.Message;
                return false;
            }
        }

        private async Task<bool> LoginWithTwitchToken()
        {
            var twitchAccessToken = await applicationStateManager.GetTwitchAccessToken().ConfigureAwait(false);
            if (twitchAccessToken == null)
                return false;

            var model = new AuthenticateWithTwitchModel { AccessToken = twitchAccessToken };
            var response = await apiGateway.Post<UserAuthenticatedModel, AuthenticateWithTwitchModel>(model, "Users", "AuthenticateWithTwitch").ConfigureAwait(false);
            if (response == null)
                return false;

            await FinalizeLogin(response).ConfigureAwait(false);

            return true;
        }

        private async Task FinalizeLogin(UserAuthenticatedModel response)
        {
            await applicationStateManager.SetJwtToken(response.Token).ConfigureAwait(false);
            User = await CreateUserFromJwtToken(response.Token).ConfigureAwait(false);
            NotifyStateChanged();
        }

        public async Task<bool> SetTokensAndLoginWithTwitch(string accessToken)
        {
            await applicationStateManager.SetTwitchAccessToken(accessToken).ConfigureAwait(false);

            return await LoginWithTwitchToken().ConfigureAwait(false);
        }
    }
}
