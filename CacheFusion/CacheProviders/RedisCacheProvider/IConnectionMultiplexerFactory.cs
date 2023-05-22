using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheFusion.CacheProviders.RedisCacheProvider
{
    public interface IConnectionMultiplexerFactory
    {
        IConnectionMultiplexer CreateConnectionMultiplexer(RedisCacheProviderOptions options);
    }
}
