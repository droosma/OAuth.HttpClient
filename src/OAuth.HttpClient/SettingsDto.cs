using System;
using System.Collections.Generic;

namespace OAuth.HttpClient
{
    public class SettingsDto
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public IEnumerable<string> Scopes { get; set; } = [];
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
