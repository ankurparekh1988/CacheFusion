using Alachisoft.NCache.Client;
using CacheFusion.CacheProviders.NCacheProvider;
using CacheFusion.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheFusion.Implementation
{
    public class NCacheProviderFactory : ICacheProviderAbstractFactory<NCacheProvider, NCacheProviderOptions>
    {
        public NCacheProvider CreateCacheProvider(NCacheProviderOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var cache = CreateCache(options);
            return new NCacheProvider(cache);
        }

        private ICache CreateCache(NCacheProviderOptions options)
        {
          
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (string.IsNullOrEmpty(options.CacheName))
            {
                throw new ArgumentNullException(nameof(options.CacheName));
            }

            CacheConnectionOptions connectionOptions = new CacheConnectionOptions
            {
                LoadBalance = options.LoadBalance
            };

            if (options.ConnectionTimeout.HasValue)
                connectionOptions.ConnectionTimeout = options.ConnectionTimeout.Value;

            if (options.ConnectionRetries.HasValue)
                connectionOptions.ConnectionRetries = options.ConnectionRetries.Value;

            if (options.EnableKeepAlive.HasValue)
                connectionOptions.EnableKeepAlive = options.EnableKeepAlive.Value;

            if (options.KeepAliveInterval.HasValue)
                connectionOptions.KeepAliveInterval = options.KeepAliveInterval.Value;

            if (options.ClientRequestTimeout.HasValue)
                connectionOptions.ClientRequestTimeOut = options.ClientRequestTimeout.Value;

            if (options.EnableClientLogs.HasValue)
                connectionOptions.EnableClientLogs = options.EnableClientLogs.Value;

            if (options.ClientBindIP != null)
                connectionOptions.ClientBindIP = options.ClientBindIP.ToString();

            // ...
            return CacheManager.GetCache(options.CacheName, connectionOptions);
        }
    }
}
