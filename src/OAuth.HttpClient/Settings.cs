using System;

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

            if(string.IsNullOrWhiteSpace(scope))
                throw new ArgumentNullException(nameof(scope));
            Scope = scope;

            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            ExpireMargin = expireMargin ?? TimeSpan.FromMinutes(5);
        }

        public ClientId ClientId { get; }
        public ClientSecret ClientSecret { get; }
        public string Scope { get; }
        public Uri Endpoint { get; }
        public TimeSpan ExpireMargin { get; }
    }
}