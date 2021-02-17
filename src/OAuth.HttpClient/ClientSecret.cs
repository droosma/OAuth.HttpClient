using System;

namespace OAuth.HttpClient
{
    public record ClientSecret
    {
        public ClientSecret(string value)
        {
            if(string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value), "client secret is required");

            Value = value;
        }

        public string Value { get; }

        public static implicit operator ClientSecret(string value) => new(value);
        public static implicit operator string(ClientSecret id) => id.Value;
    }
}