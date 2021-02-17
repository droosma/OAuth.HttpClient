using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace OAuth.HttpClient
{
    public class OAuthAuthenticator : Authentication
    {
        private readonly Action<HttpResponseMessage> _authenticationFailed;
        private readonly Action<DateTimeOffset> _fromCache;
        private readonly Func<System.Net.Http.HttpClient> _httpClientFactory;
        private readonly Action<string, DateTimeOffset> _retrieved;
        private readonly SemaphoreSlim _semaphore = new(1, 1); // Thread safe single execution of async task
        private readonly Settings _settings;
        private DateTimeOffset _expiresAt = DateTimeOffset.MinValue;
        private string _token = string.Empty;

        protected OAuthAuthenticator(Func<System.Net.Http.HttpClient> httpClientFactory,
                                     Settings settings,
                                     Action<DateTimeOffset> fromCache = null,
                                     Action<string, DateTimeOffset> retrieved = null,
                                     Action<HttpResponseMessage> authenticationFailed = null)
        {
            _httpClientFactory = httpClientFactory;
            _settings = settings;
            _fromCache = fromCache;
            _retrieved = retrieved;
            _authenticationFailed = authenticationFailed;
        }

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
                if(IsAuthenticationValid)
                {
                    _fromCache?.Invoke(_expiresAt);
                    return Value;
                }

                var (token, expires) = await Authenticate(cancellationToken).ConfigureAwait(false);
                _token = token;
                _expiresAt = Now().Add(expires).Subtract(_settings.ExpireMargin);

                _retrieved?.Invoke(_token, _expiresAt);
                return Value;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<(string Token, TimeSpan TimeToLive)> Authenticate(CancellationToken cancellationToken)
        {
            var response = await _httpClientFactory().PostAsync(_settings.Endpoint,
                                                                new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                                                                                          {
                                                                                              new("client_id", _settings.ClientId.Value),
                                                                                              new("client_secret", _settings.ClientSecret.Value),
                                                                                              new("grant_type", "client_credentials"),
                                                                                              new("scope", _settings.Scope)
                                                                                          }),
                                                                cancellationToken);
            if(!response.IsSuccessStatusCode)
            {
                _authenticationFailed?.Invoke(response);
                throw AuthenticationFailed.Create(response, _settings);
            }

            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var oAuthToken = JsonConvert.DeserializeObject<OAuthToken>(responseBody);

            return (oAuthToken.AccessToken, oAuthToken.TimeToLive);
        }

        private class OAuthToken
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; private set; }

            [JsonProperty("expires_in")]
            private int ExpiresIn { get; set; }

            public TimeSpan TimeToLive => TimeSpan.FromSeconds(ExpiresIn);
        }
    }
}