using Alachisoft.NCache.Web.Caching;
using CacheFusion.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CacheFusion.CacheProviders.NCacheProvider
{
    public class NCacheProviderOptions : ICacheProviderOptions
    {
        public string? CacheName { get; set; }

        public bool LoadBalance { get; set; } = false;

        public TimeSpan? ConnectionTimeout { get; set; }

        public int? ConnectionRetries { get; set; }

        public bool? EnableKeepAlive { get; set; }

        public TimeSpan? KeepAliveInterval { get; set; }

        public TimeSpan? ClientRequestTimeout { get; set; }

        public bool? EnableClientLogs { get; set; }

        public IPAddress? ClientBindIP { get; set; }


    }
}
