using System;

namespace OAuth.HttpClient
{
    public record ClientSecret
    {
        public string Value { get; }

        public ClientSecret(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value), "client secret is required");

            Value = value;
        }
    }
}