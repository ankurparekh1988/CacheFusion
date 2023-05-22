using Couchbase;
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.KeyValue;
using Couchbase.Query;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Couchbase.Management.Buckets;
using System.Diagnostics.Metrics;

namespace CacheFusion.CacheProviders.CouchBase
{
    public class CouchbaseCacheProvider : ICouchbaseCacheProvider
    {
        private readonly IBucket _bucket;
        private readonly ICluster _cluster;

        public CouchbaseCacheProvider(ICluster cluster, IBucket bucket)
        {
            _cluster = cluster ?? throw new ArgumentNullException(nameof(cluster));
            _bucket = bucket ?? throw new ArgumentNullException(nameof(bucket));
        }

        // ICouchbaseCacheProvider methods

        public async Task<ICouchbaseCollection> GetDefaultCollectionAsync()
        {
            return await _bucket.DefaultCollectionAsync();
        }

        public async Task<ICouchbaseCollection> GetCollectionAsync(string scopeName, string collectionName)
        {
            if (string.IsNullOrWhiteSpace(scopeName))
            {
                throw new ArgumentNullException(nameof(scopeName), "Scope name cannot be null or empty.");
            }
            if (string.IsNullOrWhiteSpace(collectionName))
            {
                throw new ArgumentNullException(nameof(collectionName), "Collection name cannot be null or empty.");
            }

            try
            {
                var scope = await _bucket.ScopeAsync(scopeName);
                return await scope.CollectionAsync(collectionName);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error getting collection '{collectionName}' in scope '{scopeName}'.", ex);
            }
        }

        

        public async Task<bool> DropIndexAsync(string indexName)
        {
            if (string.IsNullOrWhiteSpace(indexName))
            {
                throw new ArgumentNullException(nameof(indexName), "Index name cannot be null or empty.");
            }

            try
            {
                var query = $"DROP INDEX {_bucket.Name}.{indexName}";
                var result = await _bucket.Cluster.QueryAsync<dynamic>(query);
                return result.MetaData.Status == QueryStatus.Success;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error dropping index '{indexName}'.", ex);
            }
        }

        // ICacheProvider methods

        public async Task<T?> GetAsync<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }

            try
            {
                var result = await _bucket.DefaultCollectionAsync();
                var getResult = await result.GetAsync(key);
                return getResult.ContentAs<T>();
            }
            catch (DocumentNotFoundException)
            {
                return default(T);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error getting value for key '{key}'.", ex);
            }
        }

        public async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), "Value cannot be null.");
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), "Options cannot be null.");
            }

            try
            {
                var expiration = options.AbsoluteExpirationRelativeToNow ?? options.AbsoluteExpiration?.Subtract(DateTimeOffset.UtcNow);
                var json = JsonSerializer.Serialize(value);

                var upsertOptions = new UpsertOptions();
                if (expiration.HasValue)
                {
                    upsertOptions.Expiry(expiration.Value);
                }

                var collection = _bucket.DefaultCollection();
                await collection.UpsertAsync(key, json, upsertOptions);

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error setting value for key '{key}'.", ex);
            }
        }

        public async Task<bool> RemoveAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }

            try
            {
                var result = await _bucket.DefaultCollectionAsync();
                await result.RemoveAsync(key);
                return true;
            }
            catch (DocumentNotFoundException)
            {
                return false;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error removing value for key '{key}'.", ex);
            }
        }

        public async Task<bool> ContainsKeyAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }

            try
            {
                var result = await _bucket.DefaultCollectionAsync();
                await result.GetAsync(key);
                return true;
            }
            catch (DocumentNotFoundException)
            {
                return false;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error checking existence of key '{key}'.", ex);
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiresIn = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var collection = _bucket.DefaultCollection();
            var serializedValue = JsonSerializer.Serialize(value);

            if (expiresIn.HasValue)
            {
                await collection.UpsertAsync(key, serializedValue, options => options.Expiry(expiresIn.Value));
            }
            else
            {
                await collection.UpsertAsync(key, serializedValue);
            }
        }
    }
}
