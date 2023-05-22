using CacheFusion.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheFusion.CacheProviders.MemoryCacheProvider
{
    public class MemoryCacheProviderOptions : ICacheProviderOptions
    {
        public MemoryCacheOptions? MemoryCacheOptions { get; set; }
    }
}
