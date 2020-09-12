using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;

namespace AntiHarassment.TwitchIntegration
{
    public class TwitchApiWrapper : ITwitchApiIntegration
    {
        private class TwitchTokenResponse
        {
            public string Access_Token { get; set; }
            public string Refresh_Token { get; set; }
            public int Expires_In { get; set; }
            public List<string> Scope { get; set; }
            public string Token_Type { get; set; }
        }

        private class Result : ITwitchAccessTokenResult
        {
            public string TwitchUsername { get; set; }
            public string Email { get; set; }
        }

        private readonly TwitchAPI api;
        private readonly TwitchApiSettings twitchApiSettings;
        private readonly HttpClient httpClient;
        private readonly ILogger<TwitchApiWrapper> logger;

        public TwitchApiWrapper(TwitchApiSettings twitchApiSettings, HttpClient httpClient, ILogger<TwitchApiWrapper> logger)
        {
            api = new TwitchAPI();
            api.Helix.Settings.Secret = twitchApiSettings.Secret;
            api.Helix.Settings.ClientId = twitchApiSettings.ClientId;
            this.twitchApiSettings = twitchApiSettings;
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<ITwitchAccessTokenResult> GetTwitchUsernameFromToken(string accessToken)
        {
            var uri = $"https://id.twitch.tv/oauth2/token?client_id={twitchApiSettings.ClientId}&client_secret={twitchApiSettings.Secret}&code={accessToken}&grant_type=authorization_code&redirect_uri={twitchApiSettings.RedirectionUrl}";

            try
            {
                var result = await httpClient.PostAsync(uri, null).ConfigureAwait(false);
                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

                    var parsedObject = Serialization.Deserialize<TwitchTokenResponse>(content);

                    api.Helix.Settings.AccessToken = parsedObject.Access_Token;
                    api.Helix.Settings.Scopes = new List<AuthScopes> { AuthScopes.Any };

                    var userResult = await api.Helix.Users.GetUsersAsync().ConfigureAwait(false);
                    if (userResult?.Users.Length > 0)
                    {
                        var user = userResult.Users[0];
                        return new Result
                        {
                            Email = user.Email,
                            TwitchUsername = user.DisplayName
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Unable to fetch accessToken");
                return null;
            }

            return null;
        }
    }
}
