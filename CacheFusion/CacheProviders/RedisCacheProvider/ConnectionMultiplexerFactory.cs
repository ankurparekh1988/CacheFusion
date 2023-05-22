using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheFusion.CacheProviders.RedisCacheProvider
{
    public class ConnectionMultiplexerFactory : IConnectionMultiplexerFactory
    {
        public IConnectionMultiplexer CreateConnectionMultiplexer(RedisCacheProviderOptions options)
        {
            if (options == null || options.ConfigurationOptions == null)
            {
                throw new ArgumentException("Configuration options must be provided.");
            }
            var connection = ConnectionMultiplexer.Connect(options.ConfigurationOptions);
            return connection;
        }
    }
}
