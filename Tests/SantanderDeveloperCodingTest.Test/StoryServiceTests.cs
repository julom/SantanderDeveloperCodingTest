using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Legacy;
using SantanderDeveloperCodingTest.Services;
using SantanderDeveloperCodingTest.Test.TestHelpers;

namespace SantanderDeveloperCodingTest.Test
{
    public class StoryServiceTests
    {
        private MemoryCache _memoryCache;
        private Mock<IHackerNewsStoryFetcher> _fetcherMock;
        private Mock<ILogger<StoryService>> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _fetcherMock = new Mock<IHackerNewsStoryFetcher>();
            _loggerMock = new Mock<ILogger<StoryService>>();
        }

        [TearDown]
        public void TearDown()
        {
            _memoryCache.Dispose();
        }

        [Test]
        public async Task GetBestStoriesAsync_ReturnsTopNStories_AndFetchesStories()
        {
            // Arrange
            var ids = new List<int> { 1, 2, 3 };
            _fetcherMock.Setup(f => f.GetBestStoryIdListAsync()).ReturnsAsync(ids);
            _fetcherMock.Setup(f => f.GetStoryAsync(It.IsAny<int>()))
                        .ReturnsAsync((int id) => RecordStubs.StoryResponse);

            var service = new StoryService(_loggerMock.Object, _memoryCache, _fetcherMock.Object);

            // Act
            var result = (await service.GetBestStoriesAsync(2)).ToList();

            // Assert
            ClassicAssert.AreEqual(2, result.Count, "Should return the requested number of stories.");
            _fetcherMock.Verify(f => f.GetBestStoryIdListAsync(), Times.Once);
            _fetcherMock.Verify(f => f.GetStoryAsync(It.IsAny<int>()), Times.Exactly(2));
        }

        [Test]
        public void GetBestStoriesAsync_ThrowsWhenBestIdListNull()
        {
            // Arrange
            var cachedIds = (IEnumerable<int>?)null;
            var cachedKey = StoryService.CachedStoryIdListKey;
            _memoryCache.Set(cachedKey, cachedIds, TimeSpan.FromMinutes(1));
            var service = new StoryService(_loggerMock.Object, _memoryCache, _fetcherMock.Object);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => service.GetBestStoriesAsync(1));
        }

        [Test]
        public async Task GetBestStoriesAsync_UsesCache_OnlyForFirstStoryCached()
        {
            // Arrange
            var ids = new List<int> { 1, 2 };
            _fetcherMock.Setup(f => f.GetBestStoryIdListAsync()).ReturnsAsync(ids);

            // Pre-populate cache for story:1
            var cachedStory = RecordStubs.StoryResponse;
            var cachedKey = $"{StoryService.CachedStoryPrefixKey}:1";
            _memoryCache.Set(cachedKey, cachedStory, TimeSpan.FromMinutes(1));

            // For id 1, ensure fetcher is not called - throw if it is.
            _fetcherMock.Setup(f => f.GetStoryAsync(1)).Throws(new Exception("GetStoryAsync(1) should not be called when cached."));
            // For id 2, return a story normally.
            _fetcherMock.Setup(f => f.GetStoryAsync(2)).ReturnsAsync(RecordStubs.StoryResponse);

            var service = new StoryService(_loggerMock.Object, _memoryCache, _fetcherMock.Object);

            // Act
            var result = (await service.GetBestStoriesAsync(2)).ToList();

            // Assert
            ClassicAssert.AreEqual(2, result.Count, "Should return both stories (one from cache, one from fetch).");
            // Verify fetcher was never called for cached id and called once for the uncached id.
            _fetcherMock.Verify(f => f.GetStoryAsync(1), Times.Never);
            _fetcherMock.Verify(f => f.GetStoryAsync(2), Times.Once);
        }
    }
}
