using System;

namespace OAuth.HttpClient
{
    public record ClientId
    {
        public string Value { get; }

        public ClientId(string value)
        {
            if(string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value), "client id is required");
            
            Value = value;
        }
    }
}