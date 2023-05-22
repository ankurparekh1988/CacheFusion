using CacheFusion.Interfaces;
using Couchbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheFusion.CacheProviders.CouchBase
{
    public class CouchBaseProviderOptions : ICacheProviderOptions
    {
        public ClusterOptions? ClusterOptions { get; set; }

        public string? BucketName { get; set; }
    }
}
