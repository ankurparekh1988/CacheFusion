using CacheFusion.CacheProviders.CouchBase;
using Couchbase.KeyValue;
using Couchbase;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Couchbase.Query;
using Microsoft.Extensions.Caching.Distributed;

namespace CacheFusion.Test.CouchBaseTest
{
    [TestFixture]
    public class CouchbaseCacheProviderTests
    {
        private Mock<ICluster> _mockCluster;
        private Mock<IBucket> _mockBucket;
        private CouchbaseCacheProvider _cacheProvider;

        [SetUp]
        public void SetUp()
        {
            _mockCluster = new Mock<ICluster>();
            _mockBucket = new Mock<IBucket>();

            _cacheProvider = new CouchbaseCacheProvider(_mockCluster.Object, _mockBucket.Object);
        }

        [Test]
        public async Task GetDefaultCollectionAsync_ShouldReturnDefaultCollection()
        {
            var mockCollection = new Mock<ICouchbaseCollection>();
            _mockBucket.Setup(b => b.DefaultCollectionAsync()).ReturnsAsync(mockCollection.Object);

            var result = await _cacheProvider.GetDefaultCollectionAsync();

            Assert.That(result, Is.EqualTo(mockCollection.Object));
        }

        [Test]
        public async Task GetCollectionAsync_ShouldReturnCollectionWhenScopeAndCollectionNameAreValid()
        {
            var mockScope = new Mock<IScope>();
            var mockCollection = new Mock<ICouchbaseCollection>();

            _mockBucket.Setup(b => b.ScopeAsync(It.IsAny<string>())).ReturnsAsync(mockScope.Object);
            mockScope.Setup(s => s.CollectionAsync(It.IsAny<string>())).ReturnsAsync(mockCollection.Object);

            var result = await _cacheProvider.GetCollectionAsync("validScope", "validCollection");

            Assert.That(result, Is.EqualTo(mockCollection.Object));
        }

        [Test]
        public void GetCollectionAsync_ShouldThrowExceptionWhenScopeNameIsEmpty()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _cacheProvider.GetCollectionAsync(string.Empty, "validCollection"));
        }

        [Test]
        public void GetCollectionAsync_ShouldThrowExceptionWhenCollectionNameIsEmpty()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _cacheProvider.GetCollectionAsync("validScope", string.Empty));
        }

        [Test]
        public async Task DropIndexAsync_ShouldReturnTrueWhenIndexIsDroppedSuccessfully()
        {
            var mockResult = new Mock<IQueryResult<dynamic>>();
            mockResult.Setup(r => r.MetaData).Returns(new QueryMetaData { Status = QueryStatus.Success });
            _mockBucket.Setup(b => b.Cluster).Returns(_mockCluster.Object);
            _mockCluster.Setup(c => c.QueryAsync<dynamic>(It.IsAny<string>(), It.IsAny<QueryOptions>())).ReturnsAsync(mockResult.Object);

            var result = await _cacheProvider.DropIndexAsync("validIndex");

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task GetCollectionAsync_ShouldThrowExceptionWhenBucketThrowsException()
        {
            _mockBucket.Setup(b => b.ScopeAsync(It.IsAny<string>())).ThrowsAsync(new Exception("Bucket exception"));

            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _cacheProvider.GetCollectionAsync("validScope", "validCollection"));

            Assert.That(ex.Message, Does.StartWith("Error getting collection"));
            Assert.That(ex.InnerException?.Message, Is.EqualTo("Bucket exception"));
        }

        [Test]
        public async Task DropIndexAsync_ShouldThrowExceptionWhenClusterThrowsException()
        {
            _mockBucket.Setup(b => b.Cluster).Returns(_mockCluster.Object);
            _mockCluster.Setup(c => c.QueryAsync<dynamic>(It.IsAny<string>(), It.IsAny<QueryOptions>())).ThrowsAsync(new Exception("Cluster exception"));

            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _cacheProvider.DropIndexAsync("validIndex"));

            Assert.That(ex.Message, Does.StartWith("Error dropping index"));
            Assert.That(ex.InnerException?.Message, Is.EqualTo("Cluster exception"));
        }

        [Test]
        public void SetAsync_ShouldThrowExceptionWhenKeyIsEmpty()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _cacheProvider.SetAsync(string.Empty, "validValue", new DistributedCacheEntryOptions()));
        }

        

        [Test]
        public void SetAsync_ShouldThrowExceptionWhenOptionsIsNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _cacheProvider.SetAsync<string>("validKey", "validValue", (DistributedCacheEntryOptions)null));
        }

        [Test]
        public async Task SetAsync_ShouldThrowExceptionWhenBucketThrowsException()
        {
            var mockCollection = new Mock<ICouchbaseCollection>();
            _mockBucket.Setup(b => b.DefaultCollection()).Returns(mockCollection.Object);
            mockCollection.Setup(c => c.UpsertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UpsertOptions>())).ThrowsAsync(new Exception("Bucket exception"));

            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _cacheProvider.SetAsync("validKey", "validValue", new DistributedCacheEntryOptions()));

            Assert.That(ex.Message, Does.StartWith("Error setting value for key"));
            Assert.That(ex.InnerException?.Message, Is.EqualTo("Bucket exception"));
        }

        [Test]
        public async Task DropIndexAsync_ThrowsException_WhenIndexNameIsNullOrWhiteSpace()
        {
            // Arrange
            string invalidIndexName = null;

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _cacheProvider.DropIndexAsync(invalidIndexName));
            Assert.That(ex.ParamName, Is.EqualTo("indexName"));

            invalidIndexName = string.Empty;
            ex = Assert.ThrowsAsync<ArgumentNullException>(() => _cacheProvider.DropIndexAsync(invalidIndexName));
            Assert.That(ex.ParamName, Is.EqualTo("indexName"));

            invalidIndexName = "   ";
            ex = Assert.ThrowsAsync<ArgumentNullException>(() => _cacheProvider.DropIndexAsync(invalidIndexName));
            Assert.That(ex.ParamName, Is.EqualTo("indexName"));
        }

        [Test]
        public async Task GetCollectionAsync_ThrowsException_WhenScopeNameIsNullOrWhiteSpace()
        {
            // Arrange
            string invalidScopeName = null;
            string validCollectionName = "validCollectionName";

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _cacheProvider.GetCollectionAsync(invalidScopeName, validCollectionName));
            Assert.That(ex.ParamName, Is.EqualTo("scopeName"));

            invalidScopeName = string.Empty;
            ex = Assert.ThrowsAsync<ArgumentNullException>(() => _cacheProvider.GetCollectionAsync(invalidScopeName, validCollectionName));
            Assert.That(ex.ParamName, Is.EqualTo("scopeName"));

            invalidScopeName = "   ";
            ex = Assert.ThrowsAsync<ArgumentNullException>(() => _cacheProvider.GetCollectionAsync(invalidScopeName, validCollectionName));
            Assert.That(ex.ParamName, Is.EqualTo("scopeName"));
        }

        [Test]
        public async Task GetCollectionAsync_ThrowsException_WhenCollectionNameIsNullOrWhiteSpace()
        {
            // Arrange
            string validScopeName = "validScopeName";
            string invalidCollectionName = null;

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _cacheProvider.GetCollectionAsync(validScopeName, invalidCollectionName));
            Assert.That(ex.ParamName, Is.EqualTo("collectionName"));

            invalidCollectionName = string.Empty;
            ex = Assert.ThrowsAsync<ArgumentNullException>(() => _cacheProvider.GetCollectionAsync(validScopeName, invalidCollectionName));
            Assert.That(ex.ParamName, Is.EqualTo("collectionName"));

            invalidCollectionName = "   ";
            ex = Assert.ThrowsAsync<ArgumentNullException>(() => _cacheProvider.GetCollectionAsync(validScopeName, invalidCollectionName));
            Assert.That(ex.ParamName, Is.EqualTo("collectionName"));
        }

        [Test]
        public async Task ContainsKeyAsync_ThrowsException_WhenKeyIsNullOrWhiteSpace()
        {
            // Arrange
            string invalidKey = null;

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _cacheProvider.ContainsKeyAsync(invalidKey));
            Assert.That(ex.ParamName, Is.EqualTo("key"));

            invalidKey = string.Empty;
            ex = Assert.ThrowsAsync<ArgumentNullException>(() => _cacheProvider.ContainsKeyAsync(invalidKey));
            Assert.That(ex.ParamName, Is.EqualTo("key"));

            invalidKey = "   ";
            ex = Assert.ThrowsAsync<ArgumentNullException>(() => _cacheProvider.ContainsKeyAsync(invalidKey));
            Assert.That(ex.ParamName, Is.EqualTo("key"));
        }



    }


}
