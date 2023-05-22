using CacheFusion.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheFusion.CacheProviders.RedisCacheProvider
{
    public interface IRedisCacheProvider : ICacheProvider
    {
        Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiresIn = null, When when = When.Always, CommandFlags flags = CommandFlags.None);

        Task<T?> StringGetAsync<T>(string key, CommandFlags flags = CommandFlags.None);

        Task<long> StringIncrementAsync(string key, long value = 1, CommandFlags flags = CommandFlags.None);

        Task<long> StringDecrementAsync(string key, long value = 1, CommandFlags flags = CommandFlags.None);

        Task<bool> KeyExpireAsync(string key, TimeSpan? expiresIn, CommandFlags flags = CommandFlags.None);

        Task<TimeSpan?> KeyTimeToLiveAsync(string key, CommandFlags flags = CommandFlags.None);

        Task<bool> KeyDeleteAsync(string key, CommandFlags flags = CommandFlags.None);

        Task<long> ListLeftPushAsync<T>(string key, T value, When when = When.Always, CommandFlags flags = CommandFlags.None);

        Task<T?> ListLeftPopAsync<T>(string key, CommandFlags flags = CommandFlags.None);

        Task<long> ListRightPushAsync<T>(string key, T value, When when = When.Always, CommandFlags flags = CommandFlags.None);

        Task<T?> ListRightPopAsync<T>(string key, CommandFlags flags = CommandFlags.None);

        Task<long> ListLengthAsync(string key, CommandFlags flags = CommandFlags.None);

        Task<bool> HashSetAsync<T>(string key, string field, T value, When when = When.Always, CommandFlags flags = CommandFlags.None);

        Task<T?> HashGetAsync<T>(string key, string field, CommandFlags flags = CommandFlags.None);

        Task<bool> HashDeleteAsync(string key, string field, CommandFlags flags = CommandFlags.None);

        Task<bool> HashExistsAsync(string key, string field, CommandFlags flags = CommandFlags.None);

        Task<long> HashLengthAsync(string key, CommandFlags flags = CommandFlags.None);

        IDatabase GetDatabase();
    }
}

