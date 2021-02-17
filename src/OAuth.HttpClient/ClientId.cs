using System;

namespace OAuth.HttpClient
{
    public record ClientId
    {
        public ClientId(string value)
        {
            if(string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value), "client id is required");

            Value = value;
        }

        public string Value { get; }

        public static implicit operator ClientId(string value) => new(value);
        public static implicit operator string(ClientId id) => id.Value;
    }
}