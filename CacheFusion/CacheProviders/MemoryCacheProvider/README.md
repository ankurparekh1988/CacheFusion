# Memory Cache Provider

This document explains how to configure and use the `MemoryCacheProvider` in your application.

## Configuration

First, you'll need to add the `MemoryCacheProviderFactory` to your `Startup.cs` file.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<ICacheProviderAbstractFactory<IMemoryCacheProvider, MemoryCacheProviderOptions>, MemoryCacheProviderFactory>();
    
    var options = new MemoryCacheProviderOptions { MemoryCacheOptions = new MemoryCacheOptions() };
    var memoryCacheProvider = services.BuildServiceProvider().GetRequiredService<ICacheProviderAbstractFactory<IMemoryCacheProvider, MemoryCacheProviderOptions>>().CreateCacheProvider(options);
    services.AddSingleton<IMemoryCacheProvider>(memoryCacheProvider);
}

```

 ## Usage

Once you've added the `MemoryCacheProviderFactory` to your `Startup.cs` file, you can inject the `IMemoryCacheProvider` into your classes. You can use dependency injection to get an instance of IMemoryCacheProvider in your controllers or other services.

```csharp

public class MyController : ControllerBase
{
    private readonly IMemoryCacheProvider _cacheProvider;

    public MyController(IMemoryCacheProvider cacheProvider)
    {
        _cacheProvider = cacheProvider;
    }
    
    // Controller methods here...
}

```

 ## Methods

The `IMemoryCacheProvider` interface provides the following methods:

This section describes the primary methods available in the MemoryCacheProvider.

### GetAsync
Retrieves a value from the cache by its key asynchronously.

```csharp
Task<T?> GetAsync<T>(string key);
```


### SetAsync
Sets a value in the cache by a key asynchronously, with an optional expiration time.

```csharp
Task SetAsync<T>(string key, T value, TimeSpan? expiresIn = null);
```


### RemoveAsync(string key)
Removes the item associated with the key from the cache.

```csharp
Task<bool> RemoveAsync(string key);
```

### ContainsKeyAsync
Determines if the cache contains a value for the specified key asynchronously.

```csharp
Task<bool> ContainsKeyAsync(string key);
```

### GetOrCreate
Returns the existing value for a key or creates and stores a new value if it does not exist.

```csharp
T GetOrCreate<T>(string key, Func<ICacheEntry, T> factory);
```

### GetOrCreate with Options
Same as GetOrCreate but allows specifying cache entry options.

```csharp
T GetOrCreate<T>(string key, Func<ICacheEntry, T> factory, MemoryCacheEntryOptions options);
```

### Set with Options
Sets a value in the cache with additional cache entry options.

```csharp
void Set<T>(string key, T value, MemoryCacheEntryOptions options);
```

### TryGetValue
Attempts to get a value from the cache. Returns true if the value exists, false otherwise.

```csharp
bool TryGetValue<T>(string key, out T value);
```

### CreateEntry
Creates a cache entry for the specified key and returns a reference to the newly created entry.

```csharp
ICacheEntry CreateEntry(object key);
```

### Remove
Removes a value from the cache by its key.

```csharp
void Remove(object key);
```

### Compact
Compacts the memory cache by the specified percentage.

```csharp
void Compact(double percentage);
```

### GetOrCreateAsync
Asynchronous version of GetOrCreate.

```csharp
Task<T> GetOrCreateAsync<T>(string? key, Func<ICacheEntry, Task<T>> factory);
```

### GetOrCreateAsync with Options
Asynchronous version of GetOrCreate with cache entry options.

```csharp
Task<T> GetOrCreateAsync<T>(string? key, Func<ICacheEntry, Task<T>> factory, MemoryCacheEntryOptions options);
```

### SetAsync with Options
Asynchronous version of Set with cache entry options.

```csharp

Task SetAsync<T>(string key, T value, MemoryCacheEntryOptions options);
```

### TryGetValueAsync
Attempts to get a value from the cache asynchronously. Returns true if the value exists, false otherwise.

```csharp
Task<bool> TryGetValueAsync<T>(string? key, Func<Task<T>> valueFactory);
```

### CreateEntryAsync
Asynchronous version of CreateEntry.

```csharp
Task<ICacheEntry> CreateEntryAsync(object? key);
```

### CompactAsync
Asynchronous version of Compact.

```csharp
Task CompactAsync(double percentage);
```
