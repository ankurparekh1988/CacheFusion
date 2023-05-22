using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheFusion.Interfaces
{
    public interface ICacheProviderFactory
    {
        ICacheProvider CreateCacheProvider<TCacheProvider, TOptions>(TOptions options) where TCacheProvider : ICacheProvider
                                                                                                 where TOptions : ICacheProviderOptions;
    }
}
