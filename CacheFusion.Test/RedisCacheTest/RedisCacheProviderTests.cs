using CacheFusion.CacheProviders.RedisCacheProvider;
using CacheFusion.Implementation;
using Moq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheFusion.Test.RedisCacheTest
{
    [TestFixture]
    public class RedisCacheProviderTests
    {
        private Mock<IDatabase> _databaseMock;
        private Mock<IConnectionMultiplexer> _connectionMultiplexerMock;
        private Mock<IConnectionMultiplexerFactory> _connectionMultiplexerFactoryMock;
        private RedisCacheProviderFactory _redisCacheProviderFactory;
        private IRedisCacheProvider _redisCacheProvider;

        [SetUp]
        public void Setup()
        {
            _databaseMock = new Mock<IDatabase>();
            _connectionMultiplexerMock = new Mock<IConnectionMultiplexer>();
            _connectionMultiplexerMock.Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_databaseMock.Object);

            _connectionMultiplexerFactoryMock = new Mock<IConnectionMultiplexerFactory>();
            _connectionMultiplexerFactoryMock.Setup(c => c.CreateConnectionMultiplexer(It.IsAny<RedisCacheProviderOptions>())).Returns(_connectionMultiplexerMock.Object);

            _redisCacheProviderFactory = new RedisCacheProviderFactory(_connectionMultiplexerFactoryMock.Object);

            var options = new RedisCacheProviderOptions
            {
                ConfigurationOptions = new ConfigurationOptions()
            };

            _redisCacheProvider = _redisCacheProviderFactory.CreateCacheProvider(options);
        }

        [Test]
        public async Task GetAsync_ReturnsValue_WhenKeyExists()
        {
            // Arrange
            string key = "testKey";
            string expectedValue = "testValue";
            _databaseMock.Setup(db => db.StringGetAsync(key, It.IsAny<CommandFlags>()))
                         .ReturnsAsync(new RedisValue(expectedValue));

            // Act
            string result = await _redisCacheProvider.GetAsync<string>(key);

            // Assert
            Assert.That(result, Is.EqualTo(expectedValue));
        }

        [Test]
        public async Task SetAsync_SetsValue_WhenKeyDoesNotExist()
        {
            // Arrange
            string key = "testKey";
            string value = "testValue";
            _databaseMock.Setup(db => db.StringSetAsync(key, value, It.IsAny<TimeSpan?>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
                         .ReturnsAsync(true);

            // Act
            await _redisCacheProvider.SetAsync<string>(key, value);

            // Assert
            _databaseMock.Verify(db => db.StringSetAsync(key, value, It.IsAny<TimeSpan?>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
        }

        [Test]
        public async Task RemoveAsync_RemovesValue_WhenKeyExists()
        {
            // Arrange
            string key = "testKey";
            _databaseMock.Setup(db => db.KeyDeleteAsync(key, It.IsAny<CommandFlags>()))
                         .ReturnsAsync(true);

            // Act
            bool result = await _redisCacheProvider.RemoveAsync(key);

            // Assert
            Assert.IsTrue(result);
            _databaseMock.Verify(db => db.KeyDeleteAsync(key, It.IsAny<CommandFlags>()), Times.Once);
        }

        [Test]
        public async Task GetAsync_ThrowsException_WhenKeyIsNull()
        {
            // Arrange
            string key = null;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _redisCacheProvider.GetAsync<string>(key));
        }

        [Test]
        public async Task GetAsync_ThrowsException_WhenRedisExceptionOccurs()
        {
            // Arrange
            string key = "testKey";
            _databaseMock.Setup(db => db.StringGetAsync(key, It.IsAny<CommandFlags>()))
                         .ThrowsAsync(new RedisException("Error"));

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => _redisCacheProvider.GetAsync<string>(key));
        }

        [Test]
        public void SetAsync_ThrowsException_WhenKeyIsNull()
        {
            // Arrange
            string key = null;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _redisCacheProvider.SetAsync<string>(key, "testValue"));
        }

        [Test]
        public void SetAsync_ThrowsException_WhenRedisExceptionOccurs()
        {
            // Arrange
            string key = "testKey";
            string value = "testValue";
            _databaseMock.Setup(db => db.StringSetAsync(key, value, It.IsAny<TimeSpan?>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
                         .ThrowsAsync(new RedisException("Error"));

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _redisCacheProvider.SetAsync<string>(key, value));
        }

        [Test]
        public void RemoveAsync_ThrowsException_WhenKeyIsNull()
        {
            // Arrange
            string key = null;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _redisCacheProvider.RemoveAsync(key));
        }

        [Test]
        public void RemoveAsync_ThrowsException_WhenRedisExceptionOccurs()
        {
            // Arrange
            string key = "testKey";
            _databaseMock.Setup(db => db.KeyDeleteAsync(key, It.IsAny<CommandFlags>()))
                         .ThrowsAsync(new RedisException("Error"));

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _redisCacheProvider.RemoveAsync(key));
        }

        [Test]
        public async Task SetAsync_CallsStringSetAsync_WithProvidedExpiry()
        {
            // Arrange
            string key = "testKey";
            string value = "testValue";
            TimeSpan? expiresIn = TimeSpan.FromMinutes(10);

            // Act
            await _redisCacheProvider.SetAsync(key, value, expiresIn);

            // Assert
            _databaseMock.Verify(db => db.StringSetAsync(key, value, expiresIn, It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
        }


       
        [Test]
        public async Task GetAsync_WhenRedisThrowsException_ShouldThrowException()
        {
            // Arrange
            var testKey = "testKey";
            _databaseMock.Setup(db => db.StringGetAsync(testKey, It.IsAny<CommandFlags>())).ThrowsAsync(new RedisException("Redis Exception"));

            // Act and Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _redisCacheProvider.GetAsync<string>(testKey), "Error retrieving data from Redis.");
        }

        [Test]
        public async Task SetAsync_WithExpirationTime_ShouldSetExpiration()
        {
            // Arrange
            var testKey = "testKey";
            var testValue = "testValue";
            var testExpiration = TimeSpan.FromMinutes(10);

            // Act
            await _redisCacheProvider.SetAsync(testKey, testValue, testExpiration);

            // Assert
            _databaseMock.Verify(db => db.StringSetAsync(testKey, testValue, testExpiration, It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
        }

        [Test]
        public async Task SetAsync_WithSameKeyMultipleTimes_ShouldOverwritePreviousValue()
        {
            // Arrange
            var testKey = "testKey";
            var testValue1 = "testValue1";
            var testValue2 = "testValue2";

            // Act
            await _redisCacheProvider.SetAsync(testKey, testValue1);
            await _redisCacheProvider.SetAsync(testKey, testValue2);

            // Assert
            _databaseMock.Verify(db => db.StringSetAsync(testKey, testValue2, null, It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
        }

        [Test]
        public async Task RemoveAsync_WhenRedisThrowsException_ShouldThrowException()
        {
            // Arrange
            var testKey = "testKey";
            _databaseMock.Setup(db => db.KeyDeleteAsync(testKey, It.IsAny<CommandFlags>())).ThrowsAsync(new RedisException("Redis Exception"));

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _redisCacheProvider.RemoveAsync(testKey));
            
        }


    }
    

        

        
    
}
