# Memory Cache Provider

This document explains how to configure and use the `MemoryCacheProvider` in your application.

## Configuration

First, you'll need to add the `MemoryCacheProviderFactory` to your `Startup.cs` file.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<ICacheProviderAbstractFactory<IMemoryCacheProvider, MemoryCacheProviderOptions>, MemoryCacheProviderFactory>();
}

 ## Usage

Once you've added the `MemoryCacheProviderFactory` to your `Startup.cs` file, you can inject the `IMemoryCacheProvider` into your classes. You can use dependency injection to get an instance of IMemoryCacheProvider in your controllers or other services.

```csharp

public class MyController : ControllerBase
{
    private readonly IMemoryCacheProvider _cacheProvider;

    public MyController(ICacheProviderAbstractFactory<IMemoryCacheProvider, MemoryCacheProviderOptions> cacheFactory)
    {
        var options = new MemoryCacheProviderOptions();
        _cacheProvider = cacheFactory.CreateCacheProvider(options);
    }
    
    // Controller methods here...
}

 ## Methods

The `IMemoryCacheProvider` interface provides the following methods:

This section describes the primary methods available in the MemoryCacheProvider.

GetAsync<T>(string key)
Retrieves the item from the cache that's associated with this key.

SetAsync<T>(string key, T item, TimeSpan? expiry = null)
Adds an item to the cache using the specified key, with an optional expiration time.

RemoveAsync(string key)
Removes the item associated with the key from the cache.

