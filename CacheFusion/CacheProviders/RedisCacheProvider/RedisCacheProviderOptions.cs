using CacheFusion.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheFusion.CacheProviders.RedisCacheProvider
{
    public class RedisCacheProviderOptions : ICacheProviderOptions
    {
        public ConfigurationOptions? ConfigurationOptions { get; set; }
    }
}
