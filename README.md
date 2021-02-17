# OAuth.HttpClient

A DelegatingHandler implementation that abstracts away some of the authentication handshake logic like requesting the jwt token caching, and refreshing it.

[![NuGet version (ResilientOAuth.HttpClient)](https://img.shields.io/nuget/v/OAuth.HttpClient.svg?style=flat-square)](https://www.nuget.org/packages/OAuth.HttpClient/)

## Usage

```CSharp
    public void ConfigureServices(IServiceCollection services)
    {
        var settings = new Settings(clientId: "id",
                                    clientSecret: "secret",
                                    scope: "role:app",
                                    endpoint: new Uri("https://authority.local"),
                                    expireMargin: TimeSpan.FromMinutes(10));
        services.AddHttpClient("authenticatedHttpClient").WithOAuth(settings);
    }
```