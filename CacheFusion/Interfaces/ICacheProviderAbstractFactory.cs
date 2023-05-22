using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheFusion.Interfaces
{
    public interface ICacheProviderAbstractFactory<TCacheProvider, TOptions>
    where TCacheProvider : ICacheProvider
    where TOptions : ICacheProviderOptions
    {
        TCacheProvider CreateCacheProvider(TOptions options);
    }

}
