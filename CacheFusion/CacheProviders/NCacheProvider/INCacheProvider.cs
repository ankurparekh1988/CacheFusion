using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheFusion.Interfaces;

namespace CacheFusion.CacheProviders.NCacheProvider
{
    public interface INCacheProvider : ICacheProvider
    {
        Task AddAsync<T>(string key, T value, TimeSpan? expiresIn = null);
        Task<long> IncrementAsync(string key, long defaultValue, long increment);
        Task<long> DecrementAsync(string key, long defaultValue, long decrement);
        Task<(T Value, IDisposable Lock)> GetAndLockAsync<T>(string key, TimeSpan? lockTimeout = null);
        Task UnlockAsync(string key);
        Task<IDisposable> AcquireLockAsync(string key, TimeSpan? lockTimeout = null);
        Task ReleaseLockAsync(string key);
    }
}
