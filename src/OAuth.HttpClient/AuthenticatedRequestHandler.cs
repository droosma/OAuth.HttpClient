using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OAuth.HttpClient
{
    public class AuthenticatedRequestHandler : DelegatingHandler
    {
        private readonly Authentication _authentication;

        public AuthenticatedRequestHandler(Authentication authentication)
        {
            _authentication = authentication ?? throw new ArgumentNullException(nameof(authentication));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authorizationHeader = await _authentication.AuthorizationHeader(cancellationToken)
                                                           .ConfigureAwait(false);
            request.Headers.Authorization = authorizationHeader;

            return await base.SendAsync(request, cancellationToken);
        }
    }
}