using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace OAuth.HttpClient
{
    public class OAuthAuthenticator(
        Func<System.Net.Http.HttpClient> httpClientFactory,
        Settings settings,
        Action<string, DateTimeOffset>? fromCache = null,
        Action<string, DateTimeOffset>? retrieved = null,
        Action<HttpResponseMessage>? authenticationFailed = null)
        : Authentication
    {
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private DateTimeOffset _expiresAt = DateTimeOffset.MinValue;
        private string _token = string.Empty;

        public Func<DateTimeOffset> Now { private get; set; } = () => DateTimeOffset.UtcNow;

        private AuthenticationHeaderValue Value => new("Bearer", _token);

        private bool HasExpired => Now() >= _expiresAt;
        private bool HasToken => !string.IsNullOrWhiteSpace(_token);

        private bool IsAuthenticationValid => HasToken && !HasExpired;

        public async Task<AuthenticationHeaderValue> AuthorizationHeader(CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                if (IsAuthenticationValid)
                {
                    fromCache?.Invoke(_token, DateTimeOffset.MaxValue);
                    return Value;
                }

                var (token, expires) = await Authenticate(cancellationToken).ConfigureAwait(false);
                _token = token;
                _expiresAt = Now().Add(expires).Subtract(settings.ExpireMargin);

                retrieved?.Invoke(token, _expiresAt);
                return Value;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<(string, TimeSpan)> Authenticate(CancellationToken cancellationToken)
        {
            var response = await httpClientFactory().PostAsync(settings.Endpoint,
                                                               new FormUrlEncodedContent([
                                                                   new KeyValuePair<string, string>("client_id", settings.ClientId.Value),
                                                                   new KeyValuePair<string, string>("client_secret", settings.ClientSecret.Value),
                                                                   new KeyValuePair<string, string>("grant_type", "client_credentials"),
                                                                   new KeyValuePair<string, string>("scope", settings.Scope)
                                                               ]),
                                                               cancellationToken)
                                                    .ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                authenticationFailed?.Invoke(response);
                throw AuthenticationFailed.Create(response, settings);
            }

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var oAuthToken = JsonConvert.DeserializeObject<OAuthToken>(responseBody);

            if (oAuthToken is null)
            {
                authenticationFailed?.Invoke(response);
                throw AuthenticationFailed.Create(response, settings);
            }

            return (oAuthToken.AccessToken, oAuthToken.TimeToLive);
        }

        private class OAuthToken
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; private set; } = string.Empty;

            [JsonProperty("expires_in")]
            private int ExpiresIn { get; set; }

            public TimeSpan TimeToLive => TimeSpan.FromSeconds(ExpiresIn);
        }
    }
}
