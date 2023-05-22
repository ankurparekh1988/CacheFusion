using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheFusion.CacheProviders.RedisCacheProvider
{
    public class RedisCacheProvider : IRedisCacheProvider
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly IDatabase _database;

        public RedisCacheProvider(IConnectionMultiplexer connection)
        {
            _connection = connection;
            _database = _connection.GetDatabase();
        }

        // ICacheProvider methods
        public async Task<T?> GetAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }

            try
            {
                var value = await _database.StringGetAsync(key);
                if (value.IsNull) return default;

                return typeof(T).IsPrimitive
                    ? (T)Convert.ChangeType(value.ToString(), typeof(T))
                    : JsonConvert.DeserializeObject<T>(value);
            }
            catch (RedisException ex)
            {
                throw new InvalidOperationException("Error retrieving data from Redis.", ex);
            }
        }


        public async Task SetAsync<T>(string key, T value, TimeSpan? expiresIn = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }

            try
            {
                RedisValue redisValue = value is null ? RedisValue.Null
                    : typeof(T).IsPrimitive ? value.ToString() : JsonConvert.SerializeObject(value);

                if (expiresIn.HasValue)
                {
                    await _database.StringSetAsync(key, redisValue, expiresIn);
                }
                else
                {
                    await _database.StringSetAsync(key, redisValue);
                }
            }
            catch (RedisException ex)
            {
                throw new InvalidOperationException("Error setting data in Redis.", ex);
            }
        }


        public async Task<bool> ExistsAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }

            try
            {
                return await _database.KeyExistsAsync(key);
            }
            catch (RedisException ex)
            {
                throw new InvalidOperationException("Error checking key existence in Redis.", ex);
            }
        }

        public async Task<bool> RemoveAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }

            try
            {
                return await _database.KeyDeleteAsync(key);
            }
            catch (RedisException ex)
            {
                throw new InvalidOperationException("Error removing key from Redis.", ex);
            }

        }

        

        // IRedisCacheProvider methods
        public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiresIn = null, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }

            try
            {
                RedisValue redisValue = value is null ? RedisValue.Null : value.ToString();
                if (expiresIn.HasValue)
                {
                    return await _database.StringSetAsync(key, redisValue, expiresIn, when, flags);
                }
                else
                {
                    return await _database.StringSetAsync(key, redisValue, null, when, flags);
                }
            }
            catch (RedisException ex)
            {
                throw new InvalidOperationException("Error setting data in Redis.", ex);
            }
        }

        public async Task<T?> StringGetAsync<T>(string key, CommandFlags flags = CommandFlags.None)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            var value = await _database.StringGetAsync(key, flags);
            return value.IsNull ? default : (T)Convert.ChangeType(value.ToString(), typeof(T));
        }

        public Task<long> StringIncrementAsync(string key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            return _database.StringIncrementAsync(key, value, flags);
        }

        public Task<long> StringDecrementAsync(string key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            return _database.StringDecrementAsync(key, value, flags);
        }

        public Task<bool> KeyExpireAsync(string key, TimeSpan? expiresIn, CommandFlags flags = CommandFlags.None)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            return _database.KeyExpireAsync(key, expiresIn, flags);
        }

        public Task<TimeSpan?> KeyTimeToLiveAsync(string key, CommandFlags flags = CommandFlags.None)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            return _database.KeyTimeToLiveAsync(key, flags);
        }

        public Task<bool> KeyDeleteAsync(string key, CommandFlags flags = CommandFlags.None)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            return _database.KeyDeleteAsync(key, flags);
        }

        public Task<long> ListLeftPushAsync<T>(string key, T value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            RedisValue redisValue = value is null ? RedisValue.Null : value.ToString();
            return _database.ListLeftPushAsync(key, redisValue, when, flags);
        }

        public async Task<T?> ListLeftPopAsync<T>(string key, CommandFlags flags = CommandFlags.None)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            var value = await _database.ListLeftPopAsync(key, flags);
            return value.IsNull ? default : (T)Convert.ChangeType(value.ToString(), typeof(T));
        }

        public Task<long> ListRightPushAsync<T>(string key, T value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            RedisValue redisValue = value is null ? RedisValue.Null : value.ToString();
            return _database.ListRightPushAsync(key, redisValue, when, flags);
        }

        public async Task<T?> ListRightPopAsync<T>(string key, CommandFlags flags = CommandFlags.None)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            var value = await _database.ListRightPopAsync(key, flags);
            return value.IsNull ? default : (T)Convert.ChangeType(value.ToString(), typeof(T));
        }

        public Task<long> ListLengthAsync(string key, CommandFlags flags = CommandFlags.None)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            return _database.ListLengthAsync(key, flags);
        }

        public Task<bool> HashSetAsync<T>(string key, string field, T value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            RedisValue redisValue = value is null ? RedisValue.Null : value.ToString();
            return _database.HashSetAsync(key, field, redisValue, when, flags);
        }

        public async Task<T?> HashGetAsync<T>(string key, string field, CommandFlags flags = CommandFlags.None)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            var value = await _database.HashGetAsync(key, field, flags);
            return value.IsNull ? default : (T)Convert.ChangeType(value.ToString(), typeof(T));
        }

        public Task<bool> HashDeleteAsync(string key, string field, CommandFlags flags = CommandFlags.None)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            return _database.HashDeleteAsync(key, field, flags);
        }

        public Task<bool> HashExistsAsync(string key, string field, CommandFlags flags = CommandFlags.None)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            return _database.HashExistsAsync(key, field, flags);
        }

        public Task<long> HashLengthAsync(string key, CommandFlags flags = CommandFlags.None)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            return _database.HashLengthAsync(key, flags);
        }

        public IDatabase GetDatabase()
        {
            return _database;
        }

        public async Task<bool> ContainsKeyAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            }
            try
            {
                return await _database.KeyExistsAsync(key);
            }
            catch (RedisException ex)
            {
                throw new InvalidOperationException("Error checking key existence in Redis.", ex);
            }

        }

    }
}
