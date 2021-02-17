using System;
using System.Net.Http;

using Microsoft.Extensions.DependencyInjection;

namespace OAuth.HttpClient
{
    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder WithOAuth(this IHttpClientBuilder builder, Settings settings)
            => builder.WithOAuth(provider => new OAuthAuthenticator(() => provider.GetRequiredService<IHttpClientFactory>().CreateClient(), settings));

        public static IHttpClientBuilder WithOAuth(this IHttpClientBuilder builder, Authentication authentication)
            => builder.AddHttpMessageHandler(_ => new AuthenticatedRequestHandler(authentication));

        public static IHttpClientBuilder WithOAuth(this IHttpClientBuilder builder, Func<IServiceProvider, Authentication> authenticationFactory)
            => builder.AddHttpMessageHandler(provider => new AuthenticatedRequestHandler(authenticationFactory.Invoke(provider)));
    }
}