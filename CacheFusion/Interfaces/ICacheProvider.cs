using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheFusion.Interfaces
{
    public interface ICacheProvider
    {

        Task<T?> GetAsync<T>(string key);

        Task SetAsync<T>(string? key, T value, TimeSpan? expiresIn = null);

        Task<bool> RemoveAsync(string key);

        Task<bool> ContainsKeyAsync(string key);


    }

}
