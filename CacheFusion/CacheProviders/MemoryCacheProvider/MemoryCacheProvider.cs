using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheFusion.CacheProviders.MemoryCacheProvider
{
    public class MemoryCacheProvider : IMemoryCacheProvider

    {
        private readonly MemoryCache _memoryCache;

        public MemoryCacheProvider(MemoryCacheProviderOptions options)
        {
            _memoryCache = new MemoryCache(options.MemoryCacheOptions ?? new MemoryCacheOptions());
        }

        // ICacheProvider methods

        public Task<T?> GetAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            return Task.FromResult(_memoryCache.Get<T?>(key));
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiresIn = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }

            var options = expiresIn.HasValue
                ? new MemoryCacheEntryOptions().SetAbsoluteExpiration(expiresIn.Value)
                : new MemoryCacheEntryOptions();
            _memoryCache.Set(key, value, options);
            return Task.CompletedTask;
        }

        public Task<bool> RemoveAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            _memoryCache.Remove(key);
            return Task.FromResult(true);
        }

        public Task<bool> ContainsKeyAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");

            }
            return Task.FromResult(_memoryCache.TryGetValue(key, out _));
        }

        // IMemoryCacheProvider specific methods

        public T GetOrCreate<T>(string key, Func<ICacheEntry, T> factory)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            return _memoryCache.GetOrCreate(key, factory);
        }

        public T GetOrCreate<T>(string key, Func<ICacheEntry, T> factory, MemoryCacheEntryOptions options)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            return _memoryCache.GetOrCreate(key, entry =>
            {
                entry.SetOptions(options);
                return factory(entry);
            });
        }

        public void Set<T>(string key, T value, MemoryCacheEntryOptions options)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            _memoryCache.Set(key, value, options);
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            return _memoryCache.TryGetValue(key, out value);
        }

        public ICacheEntry CreateEntry(object key)
        {
            if (key == null || (key is string && string.IsNullOrEmpty(key.ToString())))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            return _memoryCache.CreateEntry(key);
        }

        public void Remove(object key)
        {
            if (key == null || (key is string && string.IsNullOrEmpty(key.ToString())))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            _memoryCache.Remove(key);
        }

        public void Compact(double percentage)
        {
            _memoryCache.Compact(percentage);
        }

        // Asynchronous versions of IMemoryCacheProvider methods

        public Task<T> GetOrCreateAsync<T>(string? key, Func<ICacheEntry, Task<T>> factory)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            return _memoryCache.GetOrCreateAsync(key, factory);
        }

        public Task<T> GetOrCreateAsync<T>(string? key, Func<ICacheEntry, Task<T>> factory, MemoryCacheEntryOptions options)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            return _memoryCache.GetOrCreateAsync(key, async entry =>
            {
                entry.SetOptions(options);
                return await factory(entry);
            });
        }

        public Task SetAsync<T>(string key, T value, MemoryCacheEntryOptions options)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            _memoryCache.Set(key, value, options);
            return Task.CompletedTask;
        }

        public async Task<bool> TryGetValueAsync<T>(string? key, Func<Task<T>> valueFactory)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            T? value = await GetAsync<T>(key);
            if (value == null)
            {
                value = await valueFactory();
                if (value != null)
                {
                    await SetAsync(key, value);
                    return true;
                }
            }
            else
            {
                return true;
            }
            return false;
        }

        public Task<ICacheEntry> CreateEntryAsync(object? key)
        {
            if (key == null || (key is string && string.IsNullOrEmpty(key.ToString())))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
             return Task.FromResult(_memoryCache.CreateEntry(key));
        }

        public Task CompactAsync(double percentage)
        {
            _memoryCache.Compact(percentage);
            return Task.CompletedTask;
        }

    }

}
