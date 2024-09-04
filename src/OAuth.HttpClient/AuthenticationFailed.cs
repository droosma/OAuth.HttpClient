using System;
using System.Net.Http;

namespace OAuth.HttpClient
{
    [Serializable]
    public class AuthenticationFailed : Exception
    {
        private AuthenticationFailed(HttpResponseMessage responseMessage,
                                     Settings settings)
            : base($"authentication failed for `{settings.ClientId}` with scope {settings.Scope} reason {responseMessage.ReasonPhrase}")
        {
        }

        public static AuthenticationFailed Create(HttpResponseMessage responseMessage,
                                                  Settings settings)
            => new(responseMessage, settings);
    }
}
