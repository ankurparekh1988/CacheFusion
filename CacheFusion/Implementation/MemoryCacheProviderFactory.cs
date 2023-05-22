using CacheFusion.CacheProviders.MemoryCacheProvider;
using CacheFusion.CacheProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;
using CacheFusion.Interfaces;

namespace CacheFusion.Implementation
{
    public class MemoryCacheProviderFactory : ICacheProviderAbstractFactory<IMemoryCacheProvider, MemoryCacheProviderOptions>
    {
        public IMemoryCacheProvider CreateCacheProvider(MemoryCacheProviderOptions options)
        {
            if (options == null || options.MemoryCacheOptions == null)
                return new MemoryCacheProvider(new MemoryCacheProviderOptions());

            return new MemoryCacheProvider(options);
        }
    }
}
