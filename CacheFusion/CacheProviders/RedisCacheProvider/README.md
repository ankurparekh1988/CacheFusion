# Redis Cache Provider

This document explains how to configure and use the `RedisCacheProvider` in your application.

## Configuration

First, you'll need to add the `RedisCacheProviderFactory` and `ConnectionMultiplexerFactory` to your `Startup.cs` file.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<IConnectionMultiplexerFactory, ConnectionMultiplexerFactory>();
    services.AddSingleton<ICacheProviderAbstractFactory<IRedisCacheProvider, RedisCacheProviderOptions>, RedisCacheProviderFactory>();
}

## Usage

Once you've configured the `RedisCacheProviderFactory` and `ConnectionMultiplexerFactory` in your `Startup.cs` file, you can inject the `ICacheProviderAbstractFactory<IRedisCacheProvider, RedisCacheProviderOptions>` into your classes and use it to create instances of the `IRedisCacheProvider`.
```csharp

public class MyController : ControllerBase
{
    private readonly IRedisCacheProvider _cacheProvider;

    public MyController(ICacheProviderAbstractFactory<IRedisCacheProvider, RedisCacheProviderOptions> cacheFactory)
    {
        var options = new RedisCacheProviderOptions
        {
            ConfigurationOptions = ConfigurationOptions.Parse("your-redis-connection-string")
        };

        _cacheProvider = cacheFactory.CreateCacheProvider(options);
    }
    
    // Controller methods here...
}

## Methods

This section describes the primary methods available in the RedisCacheProvider.

GetAsync<T>(string key)
Retrieves the item from the cache that's associated with this key.

SetAsync<T>(string key, T item, TimeSpan? expiry = null)
Adds an item to the cache using the specified key, with an optional expiration time.

RemoveAsync(string key)
Removes the item associated with the key from the cache.