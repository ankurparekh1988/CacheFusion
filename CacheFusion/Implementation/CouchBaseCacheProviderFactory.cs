using CacheFusion.CacheProviders.CouchBase;
using CacheFusion.CacheProviders.MemoryCacheProvider;
using CacheFusion.Interfaces;
using Couchbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheFusion.Implementation
{
    public class CouchBaseProviderFactory : ICacheProviderAbstractFactory<CouchbaseCacheProvider, CouchBaseProviderOptions>
    {
        private readonly ICluster _cluster;
        private readonly IBucket _bucket;

        public CouchBaseProviderFactory(ICluster cluster, IBucket bucket)
        {
            _cluster = cluster ?? throw new ArgumentNullException(nameof(cluster));
            _bucket = bucket ?? throw new ArgumentNullException(nameof(bucket));
        }



        public CouchbaseCacheProvider CreateCacheProvider(CouchBaseProviderOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (string.IsNullOrEmpty(options.BucketName))
            {
                throw new ArgumentException("CouchBaseProviderOptions must contain a valid BucketName.", nameof(options));
            }

            // Validate that the provided bucket matches the bucket used to initialize the factory
            if (_bucket.Name != options.BucketName)
            {
                throw new ArgumentException($"The provided bucket name '{options.BucketName}' does not match the bucket used to initialize the factory.", nameof(options.BucketName));
            }

            return new CouchbaseCacheProvider(_cluster, _bucket);
        }
    }
}
