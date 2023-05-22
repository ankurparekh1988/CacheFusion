using CacheFusion.CacheProviders.MemoryCacheProvider;
using CacheFusion.CacheProviders.RedisCacheProvider;
using CacheFusion.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheFusion.Implementation
{
    public class CacheProviderFactory : ICacheProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CacheProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ICacheProvider CreateCacheProvider<TCacheProvider, TOptions>(TOptions options) where TCacheProvider : ICacheProvider
                                                                                                 where TOptions : ICacheProviderOptions
        {
            // Using the service provider, we get the specific Abstract Factory
            var factory = _serviceProvider.GetRequiredService<ICacheProviderAbstractFactory<TCacheProvider, TOptions>>();

            // Using the Abstract Factory, we create and return the Cache Provider
            return factory.CreateCacheProvider(options);
        }

    }
}
