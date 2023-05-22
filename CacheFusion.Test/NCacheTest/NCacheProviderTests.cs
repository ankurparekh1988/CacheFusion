using Alachisoft.NCache.Client;
using CacheFusion.CacheProviders.NCacheProvider;
using Moq;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CacheFusion.Test.NCacheTest
{
    [TestFixture]
    public class NCacheProviderTests
    {
        private NCacheProvider _ncacheProvider;
        private Mock<ICache> _mockCache;

        [SetUp]
        public void Setup()
        {
            // Initialize the mock object
            _mockCache = new Mock<ICache>();

            // Initialize the NCacheProvider with the mock object
            _ncacheProvider = new NCacheProvider(_mockCache.Object);
        }

        [Test]
        public void AddAsync_NullKey_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _ncacheProvider.AddAsync<string>(null, "testValue"));
        }

        [Test]
        public void AddAsync_NullValue_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _ncacheProvider.AddAsync("testKey", null as string));
        }

        [Test]
        public async Task AddAsync_ValidInput_CallsInsertAsyncOnCache()
        {
            string testKey = "testKey";
            string testValue = "testValue";

            await _ncacheProvider.AddAsync(testKey, testValue);

            _mockCache.Verify(c => c.InsertAsync(It.Is<string>(s => s == testKey), It.IsAny<CacheItem>(), null), Times.Once);
        }

        [Test]
        public void AddAsync_EmptyKey_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _ncacheProvider.AddAsync(string.Empty, "testValue"));
        }


        [Test]
        public void ContainsKeyAsync_EmptyKey_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _ncacheProvider.ContainsKeyAsync(string.Empty));
        }

        [Test]
        public async Task ContainsKeyAsync_KeyExists_ReturnsTrue()
        {
            string testKey = "testKey";

            _mockCache.Setup(c => c.Contains(It.IsAny<string>())).Returns(true);

            var result = await _ncacheProvider.ContainsKeyAsync(testKey);

            Assert.IsTrue(result);
        }

        [Test]
        public void GetAsync_EmptyKey_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _ncacheProvider.GetAsync<string>(string.Empty));
        }

    }
}
