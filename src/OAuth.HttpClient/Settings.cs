using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Reflection.Metadata;

namespace OAuth.HttpClient
{
    public class Settings
    {
        public Settings(ClientId clientId,
                        ClientSecret clientSecret,
                        string scope,
                        Uri endpoint,
                        TimeSpan? expireMargin = null)
        {
            ClientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
            ClientSecret = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret));

            if (string.IsNullOrWhiteSpace(scope))
                throw new ArgumentNullException(nameof(scope));
            Scope = scope;

            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            ExpireMargin = expireMargin ?? TimeSpan.FromMinutes(5);
        }

        public ClientId ClientId { get; }
        public ClientSecret ClientSecret { get; }
        public string Scope { get; }
        public Uri Endpoint { get; }

        /// <summary>
        /// The margin used for refreshing the token before given expiry time
        /// </summary>
        public TimeSpan ExpireMargin { get; }
    }

    public class SettingsDto
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public IEnumerable<string> Scopes { get; set; } = Array.Empty<string>();
        public string TokenEndpoint { get; set; } = string.Empty;

        /// <summary>
        /// The margin in seconds used for refreshing the token before given expiry time
        /// </summary>
        public int? ExpireMarginInSeconds { get; } = 30;

        public static implicit operator Settings(SettingsDto dto)
            => new(
                dto.ClientId,
                dto.ClientSecret,
                string.Join(" ", dto.Scopes),
                new Uri(dto.TokenEndpoint),
                dto.ExpireMarginInSeconds.HasValue ? TimeSpan.FromSeconds(dto.ExpireMarginInSeconds.Value) : null
                );
    }
}