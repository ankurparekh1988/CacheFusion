using Alachisoft.NCache.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheFusion.CacheProviders.NCacheProvider
{
    public class LockHandleWrapper : IDisposable
    {
        private readonly ICache _cache;
        private readonly string _key;
        private readonly LockHandle _lockHandle;

        public LockHandleWrapper(ICache cache, string key, LockHandle lockHandle)
        {
            _cache = cache;
            _key = key;
            _lockHandle = lockHandle;
        }

        public void Dispose()
        {
            _cache.Unlock(_key, _lockHandle);
        }
    }
}
