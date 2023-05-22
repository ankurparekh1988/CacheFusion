using Alachisoft.NCache.Client;
using Alachisoft.NCache.Common.Locking;
using Alachisoft.NCache.Common;
using Alachisoft.NCache.Runtime;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Alachisoft.NCache.Common.ErrorHandling;

namespace CacheFusion.CacheProviders.NCacheProvider
{
    public class NCacheProvider : INCacheProvider
    {
        private readonly ICache _cache;

        public NCacheProvider(ICache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task AddAsync<T>(string key, T value, TimeSpan? expiresIn = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            try
            {
                Alachisoft.NCache.Client.CacheItem cacheItem = new Alachisoft.NCache.Client.CacheItem(value);

                if (expiresIn.HasValue)
                {
                    cacheItem.Expiration = new Expiration(ExpirationType.Absolute, expiresIn.Value);
                }

                await _cache.AddAsync(key, cacheItem);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while adding the item with key '{key}' to NCache.", ex);
            }
        }

        public async Task<long> IncrementAsync(string key, long defaultValue, long increment)
        {

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            try
            {
                // Ensure the key exists with defaultValue before incrementing
                if (!_cache.Contains(key))
                {
                    await _cache.AddAsync(key, defaultValue);
                }

                // Perform the increment operation
                var result = await Task.Run(() => _cache.DataTypeManager.GetCounter(key).IncrementBy(increment));
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while incrementing the value with key '{key}' in NCache.", ex);
            }
        }


        public async Task<bool> ContainsKeyAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            try
            {
                // Check if the key exists in the cache
                bool exists = await Task.Run(() => _cache.Contains(key));
                return exists;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while checking the existence of the key '{key}' in NCache.", ex);
            }
        }

        public async Task<long> DecrementAsync(string key, long defaultValue, long decrement)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            try
            {
                // Ensure the key exists with defaultValue before incrementing
                if (!_cache.Contains(key))
                {
                    await _cache.AddAsync(key, defaultValue);
                }

                // Perform the decrement operation
                var result = await Task.Run(() => _cache.DataTypeManager.GetCounter(key).DecrementBy(decrement));
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while decrementing the value with key '{key}' in NCache.", ex);
            }
        }

        public async Task<(T Value, IDisposable Lock)> GetAndLockAsync<T>(string key, TimeSpan? lockTimeout = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            try
            {
                LockHandle lockHandle = new LockHandle();
                TimeSpan lockTimeoutValue = lockTimeout ?? TimeSpan.FromSeconds(30);

                bool isLocked = await Task.Run(() => _cache.Lock(key, lockTimeoutValue, out lockHandle));

                if (isLocked)
                {
                    var value = await Task.Run(() => _cache.Get<T>(key));
                    return (value, new LockHandleWrapper(_cache, key, lockHandle));
                }
                else
                {
                    throw new InvalidOperationException($"Failed to acquire lock on the item with key '{key}' in NCache.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while getting and locking the item with key '{key}' in NCache.", ex);
            }
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }

            T? result = default;

            try
            {
                await Task.Run(() =>
                {
                    var cacheItem = _cache.Get<T>(key);
                    if (cacheItem != null)
                    {
                        result = cacheItem;
                    }
                });
            }
            catch (Exception ex)
            {
                // You may want to log the exception here, depending on your logging strategy
                throw new InvalidOperationException($"An error occurred while attempting to retrieve the value for key '{key}' from the NCache instance.", ex);
            }

            return result;

        }



        public async Task ReleaseLockAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }

            try
            {
                await Task.Run(() =>
                {
                    _cache.Unlock(key);
                });
            }
            catch (OperationFailedException ex)
            {
                if (ex.ErrorCode == NCacheErrorCodes.ITEM_LOCKED)
                {
                    throw new InvalidOperationException($"The key '{key}' is locked by another client.", ex);
                }
                else if (ex.Message.Contains("not locked") || ex.Message.Contains("lock has already been released"))
                {
                    throw new InvalidOperationException($"The key '{key}' is not locked or the lock has already been released.", ex);
                }
                else
                {
                    // You may want to log the exception here, depending on your logging strategy
                    throw new InvalidOperationException($"An error occurred while attempting to release the lock for key '{key}' in the NCache instance.", ex);
                }
            }
            catch (Exception ex)
            {
                // You may want to log the exception here, depending on your logging strategy
                throw new InvalidOperationException($"An error occurred while attempting to release the lock for key '{key}' in the NCache instance.", ex);
            }
        }

        public async Task<bool> RemoveAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }

            bool removed = false;

            try
            {
                var removedCacheItem = await _cache.RemoveAsync<Alachisoft.NCache.Client.CacheItem>(key);
                removed = removedCacheItem != null;
            }
            catch (OperationFailedException ex)
            {
                throw new InvalidOperationException($"An error occurred while attempting to remove the key '{key}' from the NCache instance.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while attempting to remove the key '{key}' from the NCache instance.", ex);
            }

            return removed;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiresIn = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), "Value cannot be null.");
            }

            try
            {
                var cacheItem = new Alachisoft.NCache.Client.CacheItem(value);

                if (expiresIn.HasValue)
                {
                    cacheItem.Expiration = new Alachisoft.NCache.Runtime.Caching.Expiration(Alachisoft.NCache.Runtime.Caching.ExpirationType.Absolute, expiresIn.Value);
                }

                await _cache.InsertAsync(key, cacheItem);
            }
            catch (OperationFailedException ex)
            {
                throw new InvalidOperationException($"An error occurred while attempting to set the value for the key '{key}' in the NCache instance.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while attempting to set the value for the key '{key}' in the NCache instance.", ex);
            }
        }

        public async Task UnlockAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }

            await Task.Run(() =>
            {
                try
                {
                    var lockHandle = new LockHandle();
                    var lockTimeout = TimeSpan.FromSeconds(0);
                    var acquired = _cache.Lock(key, lockTimeout, out lockHandle);

                    if (acquired)
                    {
                        _cache.Unlock(key, lockHandle);
                    }
                    else
                    {
                        throw new InvalidOperationException($"The key '{key}' is not locked or has an expired lock.");
                    }
                }
                catch (OperationFailedException ex)
                {
                    if (!ex.Message.Contains("Item not found"))
                    {
                        throw new InvalidOperationException($"Failed to unlock the key '{key}'.", ex);
                    }
                    else
                    {
                        throw new InvalidOperationException($"The key '{key}' does not exist in the cache.", ex);
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"An unexpected error occurred while unlocking the key '{key}'.", ex);
                }
            });
        }

        public async Task<IDisposable> AcquireLockAsync(string key, TimeSpan? lockTimeout = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }

            return await Task.Run(() =>
            {
                try
                {
                    var lockHandle = new LockHandle();
                    var effectiveLockTimeout = lockTimeout ?? TimeSpan.FromSeconds(0);
                    bool acquired = _cache.Lock(key, effectiveLockTimeout, out lockHandle);

                    if (acquired)
                    {
                        return (IDisposable)new LockHandleWrapper(_cache, key, lockHandle);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Could not acquire lock for key '{key}'.");
                    }
                }
                catch (OperationFailedException ex)
                {
                    throw new InvalidOperationException($"Failed to acquire lock for key '{key}'.", ex);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"An unexpected error occurred while trying to acquire lock for key '{key}'.", ex);
                }
            });
        }
    }

    
}
