UserClient
--------------------------------------------------------------------------------------------------------------------------------------------

A .NET 7+ class library and console application to fetch user data from a public API, featuring:

Clean Architecture: Separation of Core, Infrastructure, and Application layers.

HttpClientFactory + Polly: Typed ExternalUserService with retry policy for resilience.

Configuration: Strongly-typed options pattern (ApiSettings, CacheSettings).

In-Memory Caching: Configurable expiration for list and individual user calls.

Unit Tests: xUnit tests for service behavior.

Folder Structure
--------------------------------------------------------------------------------------------------------------------------------------------

/UserClient.sln

/src
 ├─ UserClient.Core             # Models & Interfaces
 ├─ UserClient.Infrastructure   # Implementation, Policies, Extensions
 └─ UserClient.ConsoleApp       # Demonstration console application

tests
 └─ UserClient.Tests            # Unit tests

 Prerequisites
 --------------------------------------------------------------------------------------------------------------------------------------------

.NET 7 SDK (or later)

Optional: Visual Studio 2022+, VS Code, JetBrains Rider

Internet access for hitting https://reqres.in/api

Configuration
--------------------------------------------------------------------------------------------------------------------------------------------
1. In ConsoleApp, ensure appsettings.json exists:
{
  "ApiSettings": {
    "BaseUrl": "https://reqres.in/api",
    "ApiKey": <if needed>,
    "ApiKeyHeaderName": <if needed>
  },
  "CacheSettings": {
    "UserCacheDurationInMinutes": <configure>
  }
}

2.Ensure the appsettings.json file is copied to output dir if newer.


Design Decisions
--------------------------------------------------------------------------------------------------------------------------------------------

Clean Architecture keeps domain models in Core, implementation details in Infrastructure, and no cross-dependencies.

HttpClientFactory: Ensures efficient socket reuse and integrates Polly policies seamlessly.

Polly Retry: Exponential backoff (2s, 4s, 8s) on transient failures.

In-Memory Cache: TTL-based caching.

Options Pattern: IOptions<ApiSettings> and IOptions<CacheSettings> for flexible configuration.
