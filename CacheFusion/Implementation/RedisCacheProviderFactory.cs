using CacheFusion.CacheProviders.RedisCacheProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheFusion.Interfaces;
using CacheFusion.CacheProviders.MemoryCacheProvider;
using StackExchange.Redis;

namespace CacheFusion.Implementation
{
    public class RedisCacheProviderFactory : ICacheProviderAbstractFactory<IRedisCacheProvider, RedisCacheProviderOptions>
    {
        private readonly IConnectionMultiplexerFactory _connectionMultiplexerFactory;

        public RedisCacheProviderFactory(IConnectionMultiplexerFactory connectionMultiplexerFactory)
        {
            _connectionMultiplexerFactory = connectionMultiplexerFactory;
        }

        public IRedisCacheProvider CreateCacheProvider(RedisCacheProviderOptions options)
        {
            if (options == null || options.ConfigurationOptions == null)
            {
                throw new ArgumentException("Configuration options must be provided.");
            }

            var connectionMultiplexer = _connectionMultiplexerFactory.CreateConnectionMultiplexer(options);

            return new RedisCacheProvider(connectionMultiplexer);
        }
    }
}
