# Redis Cache Provider

This document explains how to configure and use the `RedisCacheProvider` in your application.

## Configuration

First, you'll need to add the `RedisCacheProviderFactory` and `ConnectionMultiplexerFactory` to your `Startup.cs` file.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<IConnectionMultiplexerFactory, ConnectionMultiplexerFactory>();
    services.AddSingleton<ICacheProviderAbstractFactory<IRedisCacheProvider, RedisCacheProviderOptions>, RedisCacheProviderFactory>();
    
    var options = new RedisCacheProviderOptions
    {
        ConfigurationOptions = ConfigurationOptions.Parse("your-redis-connection-string")
    };
    var redisCacheProvider = services.BuildServiceProvider().GetRequiredService<ICacheProviderAbstractFactory<IRedisCacheProvider, RedisCacheProviderOptions>>().CreateCacheProvider(options);
    services.AddSingleton<IRedisCacheProvider>(redisCacheProvider);
}
```

## Usage

Once you've configured the `RedisCacheProviderFactory` and `ConnectionMultiplexerFactory` in your `Startup.cs` file, you can inject the `ICacheProviderAbstractFactory<IRedisCacheProvider, RedisCacheProviderOptions>` into your classes and use it to create instances of the `IRedisCacheProvider`.

```csharp

public class MyController : ControllerBase
{
    private readonly IRedisCacheProvider _cacheProvider;

    public MyController(IRedisCacheProvider cacheProvider)
    {
        _cacheProvider = cacheProvider;
    }
    
    // Controller methods here...
}

```

## Methods

This section describes the primary methods available in the RedisCacheProvider.

### GetAsync

Gets the value of the key as the specified type. 

Usage:

```csharp
Task<T?> GetAsync<T>(string key)
```

### SetAsync

Sets the value of the key.

Usage:

```csharp
Task SetAsync<T>(string key, T value, TimeSpan? expiresIn = null)
Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiresIn = null, When when = When.Always, CommandFlags flags = CommandFlags.None)
```

### ExistsAsync

Checks if the given key exists.

Usage:

```csharp
Task<bool> ExistsAsync(string key)
```

### RemoveAsync

Deletes the specified key.

Usage:

```csharp
Task<bool> RemoveAsync(string key)
```

### StringGetAsync

Gets the value of the key.

Usage:

```csharp
Task<T?> StringGetAsync<T>(string key, CommandFlags flags = CommandFlags.None)
```

### StringIncrementAsync

Increments the value of the key by the specified amount.

Usage:

```csharp
Task<long> StringIncrementAsync(string key, long value = 1, CommandFlags flags = CommandFlags.None)
```

### StringDecrementAsync

Decrements the value of the key by the specified amount.

Usage:

```csharp
Task<long> StringDecrementAsync(string key, long value = 1, CommandFlags flags = CommandFlags.None)
```

### KeyExpireAsync

Sets the expiration of the key.

Usage:

```csharp
Task<bool> KeyExpireAsync(string key, TimeSpan? expiresIn, CommandFlags flags = CommandFlags.None)
```

### KeyTimeToLiveAsync

Gets the remaining time to live of a key that has a timeout.

Usage:

```csharp
Task<TimeSpan?> KeyTimeToLiveAsync(string key, CommandFlags flags = CommandFlags.None)
```

### KeyDeleteAsync

Deletes the specified key.

Usage:

```csharp
Task<bool> KeyDeleteAsync(string key, CommandFlags flags = CommandFlags.None)
```

### ListLeftPushAsync

Inserts the specified value at the head of the list stored at key.

Usage:

```csharp
Task<long> ListLeftPushAsync<T>(string key, T value, When when = When.Always, CommandFlags flags = CommandFlags.None)
```

### ListLeftPopAsync

Removes and returns the first element of the list stored at key.

Usage:

```csharp
Task<T?> ListLeftPopAsync<T>(string key, CommandFlags flags = CommandFlags.None)
```

### ListRightPushAsync

Inserts the specified value at the tail of the list stored at key.

Usage:

```csharp
Task<long> ListRightPushAsync<T>(string key, T value, When when = When.Always, CommandFlags flags = CommandFlags.None)
```

### ListRightPopAsync

Removes and returns the last element of the list stored at key.

Usage:

```csharp
Task<T?> ListRightPopAsync<T>(string key, CommandFlags flags = CommandFlags.None)
```

### ListLengthAsync

Returns the length of the list stored at key.

Usage:

```csharp
Task<long> ListLengthAsync(string key, CommandFlags flags = CommandFlags.None)
```

### HashSetAsync

Sets field in the hash stored at key to value.

Usage:

```csharp
Task<bool> HashSetAsync<T>(string key, string field, T value, When when = When.Always, CommandFlags flags = CommandFlags.None)
```

### HashGetAsync

Returns the value associated with field in the hash stored at key.

Usage:

```csharp
Task<T?> HashGetAsync<T>(string key, string field, CommandFlags flags = CommandFlags.None)
```

### HashDeleteAsync

Deletes one or more hash fields.

Usage:

```csharp
Task<bool> HashDeleteAsync(string key, string field, CommandFlags flags = CommandFlags.None)
```

### HashExistsAsync

Returns if field is an existing field in the hash stored at key.

Usage:

```csharp
Task<bool> HashExistsAsync(string key, string field, CommandFlags flags = CommandFlags.None)
```

### HashLengthAsync

Returns the number of fields contained in the hash stored at key.

Usage:

```csharp
Task<long> HashLengthAsync(string key, CommandFlags flags = CommandFlags.None)
```

### GetDatabase

Gets the Redis Database instance.

Usage:

```csharp
IDatabase GetDatabase()
```

### ContainsKeyAsync

Checks if the given key exists.

Usage:

```csharp
Task<bool> ContainsKeyAsync(string key)
```