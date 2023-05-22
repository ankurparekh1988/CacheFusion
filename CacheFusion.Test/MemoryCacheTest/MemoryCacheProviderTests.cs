using CacheFusion.CacheProviders.MemoryCacheProvider;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheFusion.Test.MemoryCacheTest
{
    [TestFixture]
    public class MemoryCacheProviderTests
    {
        private IMemoryCacheProvider _memoryCacheProvider;
        private MemoryCacheProviderOptions _options;

        [SetUp]
        public void Setup()
        {
            _options = new MemoryCacheProviderOptions
            {
                MemoryCacheOptions = new MemoryCacheOptions()
            };
            _memoryCacheProvider = new MemoryCacheProvider(_options);
        }

        [Test]
        public async Task SetAndGetAsync_ShouldWorkCorrectly()
        {
            string key = "testKey";
            string value = "testValue";

            await _memoryCacheProvider.SetAsync(key, value);
            var retrievedValue = await _memoryCacheProvider.GetAsync<string>(key);

            Assert.That(retrievedValue, Is.EqualTo(value));
        }

        [Test]
        public async Task ContainsKeyAsync_ShouldReturnTrue_IfKeyExists()
        {
            string key = "testKey";
            string value = "testValue";

            await _memoryCacheProvider.SetAsync(key, value);
            var containsKey = await _memoryCacheProvider.ContainsKeyAsync(key);

            Assert.IsTrue(containsKey);
        }

        [Test]
        public async Task ContainsKeyAsync_ShouldReturnFalse_IfKeyNotExists()
        {
            string key = "testKey";

            var containsKey = await _memoryCacheProvider.ContainsKeyAsync(key);

            Assert.IsFalse(containsKey);
        }

        [Test]
        public async Task RemoveAsync_ShouldRemoveTheKey()
        {
            string key = "testKey";
            string value = "testValue";

            await _memoryCacheProvider.SetAsync(key, value);
            await _memoryCacheProvider.RemoveAsync(key);
            var containsKey = await _memoryCacheProvider.ContainsKeyAsync(key);

            Assert.IsFalse(containsKey);
        }

        [Test]
        public void SetAsync_ShouldThrowException_WhenKeyIsNull()
        {
            string? key = null;
            string value = "testValue";

            var ex = Assert.Throws<ArgumentNullException>(() => _memoryCacheProvider.SetAsync(key, value));

            StringAssert.Contains("Key cannot be null or empty.", ex.Message);
        }

        [Test]
        public async Task GetAsync_ShouldReturnNull_WhenKeyDoesNotExist()
        {
            string key = "nonexistentKey";

            var value = await _memoryCacheProvider.GetAsync<string>(key);

            Assert.IsNull(value);
        }

        [Test]
        public void GetOrCreate_ShouldReturnExistingValue_WhenKeyExists()
        {
            string key = "testKey";
            string value = "testValue";

            _memoryCacheProvider.Set(key, value, new MemoryCacheEntryOptions());
            var result = _memoryCacheProvider.GetOrCreate(key, entry => "newTestValue");

            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        public void GetOrCreate_ShouldReturnNewValue_WhenKeyDoesNotExist()
        {
            string key = "testKey";
            string value = "testValue";

            var result = _memoryCacheProvider.GetOrCreate(key, entry => value);

            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        public void TryGetValue_ShouldReturnTrueAndOutTheValue_WhenKeyExists()
        {
            string key = "testKey";
            string value = "testValue";

            _memoryCacheProvider.Set(key, value, new MemoryCacheEntryOptions());
            var result = _memoryCacheProvider.TryGetValue(key, out string outValue);

            Assert.IsTrue(result);
            Assert.That(outValue, Is.EqualTo(value));
        }

        [Test]
        public void TryGetValue_ShouldReturnFalseAndOutNull_WhenKeyDoesNotExist()
        {
            string key = "nonexistentKey";

            var result = _memoryCacheProvider.TryGetValue<string>(key, out string outValue);

            Assert.IsFalse(result);
            Assert.IsNull(outValue);
        }

        [Test]
        public void Compact_ShouldReduceCacheSize()
        {
            for (int i = 0; i < 100; i++)
            {
                _memoryCacheProvider.Set($"key{i}", $"value{i}", new MemoryCacheEntryOptions());
            }

            _memoryCacheProvider.Compact(0.5);

            for (int i = 0; i < 50; i++)
            {
                var result = _memoryCacheProvider.TryGetValue<string>($"key{i}", out string outValue);

                Assert.IsFalse(result);
                Assert.IsNull(outValue);
            }
        }

        [Test]
        public async Task RemoveAsync_ShouldReturnTrue_WhenKeyExists()
        {
            string key = "testKey";
            string value = "testValue";

            await _memoryCacheProvider.SetAsync(key, value);
            bool result = await _memoryCacheProvider.RemoveAsync(key);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task RemoveAsync_ShouldReturnFalse_WhenKeyDoesNotExist()
        {
            string key = "testKey";

            bool result = await _memoryCacheProvider.RemoveAsync(key);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task ContainsKeyAsync_ShouldReturnTrue_WhenKeyExists()
        {
            string key = "testKey";
            string value = "testValue";

            await _memoryCacheProvider.SetAsync(key, value);
            bool result = await _memoryCacheProvider.ContainsKeyAsync(key);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task ContainsKeyAsync_ShouldReturnFalse_WhenKeyDoesNotExist()
        {
            string key = "testKey";

            bool result = await _memoryCacheProvider.ContainsKeyAsync(key);

            Assert.IsFalse(result);
        }

        [Test]
        public void Set_ShouldThrowException_WhenKeyIsNull()
        {
            string? key = null;
            string value = "testValue";

            var ex = Assert.Throws<ArgumentNullException>(() => _memoryCacheProvider.Set(key, value, new MemoryCacheEntryOptions()));

            StringAssert.Contains("Key cannot be null or empty.", ex.Message);
        }

        [Test]
        public void TryGetValue_ShouldThrowException_WhenKeyIsNull()
        {
            string? key = null;

            var ex = Assert.Throws<ArgumentNullException>(() => _memoryCacheProvider.TryGetValue<string>(key, out _));

            StringAssert.Contains("Key cannot be null or empty.", ex.Message);
        }

        [Test]
        public void GetOrCreate_ShouldThrowException_WhenKeyIsNull()
        {
            string? key = null;

            var ex = Assert.Throws<ArgumentNullException>(() => _memoryCacheProvider.GetOrCreate(key, entry => "value"));

            StringAssert.Contains("Key cannot be null or empty.", ex.Message);
        }

        [Test]
        public async Task SetAsync_ShouldThrowException_WhenKeyIsNull_AndMemoryCacheEntryOptionsIsNotNull()
        {
            string? key = null;
            string value = "testValue";
            var options = new MemoryCacheEntryOptions();

            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _memoryCacheProvider.SetAsync(key, value, options));

            StringAssert.Contains("Key cannot be null or empty.", ex.Message);
        }

        [Test]
        public void GetOrCreateAsync_ShouldThrowException_WhenKeyIsNull()
        {
            string? key = null;

            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _memoryCacheProvider.GetOrCreateAsync(key, entry => Task.FromResult("value")));

            StringAssert.Contains("Key cannot be null or empty.", ex.Message);
        }

        [Test]
        public void GetOrCreateAsync_ShouldThrowException_WhenKeyIsNull_AndMemoryCacheEntryOptionsIsNotNull()
        {
            string? key = null;
            var options = new MemoryCacheEntryOptions();

            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _memoryCacheProvider.GetOrCreateAsync(key, entry => Task.FromResult("value"), options));

            StringAssert.Contains("Key cannot be null or empty.", ex.Message);
        }

        [Test]
        public void TryGetValueAsync_ShouldThrowException_WhenKeyIsNull()
        {
            string? key = null;

            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _memoryCacheProvider.TryGetValueAsync(key, () => Task.FromResult("value")));

            StringAssert.Contains("Key cannot be null or empty.", ex.Message);
        }

        [Test]
        public void CreateEntryAsync_ShouldThrowException_WhenKeyIsNull()
        {
            object? key = null;

            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _memoryCacheProvider.CreateEntryAsync(key));

            StringAssert.Contains("Key cannot be null or empty.", ex.Message);
        }

        [Test]
        public async Task TryGetValueAsync_ShouldReturnTrueAndSetValue_WhenKeyDoesNotExist()
        {
            string key = "testKey";
            string value = "testValue";

            bool result = await _memoryCacheProvider.TryGetValueAsync(key, () => Task.FromResult(value));

            Assert.IsTrue(result);

            string? retrievedValue = await _memoryCacheProvider.GetAsync<string>(key);
            Assert.That(retrievedValue, Is.EqualTo(value));
        }


    }

}
