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

## Configuration

To facilitate configuration from IConfiguration you can use the `SettingsDto` to create the required settings directly from `IConfiguration`

Add the required settings to your appsettings.json

```Json
{
  "OAuthSettings": {
    "ClientId": "client_id",
    "ClientSecret": "client_secret",
    "Scopes": ["api1", "api2"],
    "TokenEndpoint": "https://localhost:5001/connect/token"
  }
}
```

Initialize the client with the correct settings (or any other ConfigurationSource)

```CSharp
var settings = hostBuilder.Configuration.GetRequiredSection("OAuthSettings").Get<SettingsDto>();
hostBuilder.Services.AddHttpClient("authenticatedHttpClient").WithOAuth(settings);
```
