using CacheFusion.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheFusion.CacheProviders.MemoryCacheProvider
{
    public interface IMemoryCacheProvider : ICacheProvider
    {
        // Synchronous Methods
        T GetOrCreate<T>(string key, Func<ICacheEntry, T> factory);

        T GetOrCreate<T>(string key, Func<ICacheEntry, T> factory, MemoryCacheEntryOptions options);

        void Set<T>(string key, T value, MemoryCacheEntryOptions options);

        bool TryGetValue<T>(string key, out T value);

        ICacheEntry CreateEntry(object key);

        void Remove(object key);

        void Compact(double percentage);

        // Asynchronous Methods
        Task<T> GetOrCreateAsync<T>(string? key, Func<ICacheEntry, Task<T>> factory);

        Task<T> GetOrCreateAsync<T>(string? key, Func<ICacheEntry, Task<T>> factory, MemoryCacheEntryOptions options);

        Task SetAsync<T>(string key, T value, MemoryCacheEntryOptions options);

        Task<bool> TryGetValueAsync<T>(string? key, Func<Task<T>> valueFactory);

        Task<ICacheEntry> CreateEntryAsync(object? key);

        Task CompactAsync(double percentage);
    }
}
