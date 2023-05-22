using CacheFusion.Interfaces;
using Couchbase.KeyValue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheFusion.CacheProviders.CouchBase
{
    public interface ICouchbaseCacheProvider : ICacheProvider
    {
        Task<ICouchbaseCollection> GetDefaultCollectionAsync();
        Task<ICouchbaseCollection> GetCollectionAsync(string scopeName, string collectionName);
        
        Task<bool> DropIndexAsync(string indexName);
    }
}
