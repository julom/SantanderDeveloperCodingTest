using Microsoft.Extensions.Caching.Memory;
using SantanderDeveloperCodingTest.Dtos;
using SantanderDeveloperCodingTest.HttpClients;

namespace SantanderDeveloperCodingTest.Providers
{
    public class StoryProvider(
        ILogger<StoryProvider> logger,
        IMemoryCache memoryCache,
        IHackerNewsStoryHttpClient storyHttpClient) : IStoryProvider
    {
        public const string CachedStoryIdListKey = "full_list_of_story_ids";
        private const int CacheStoryIdListExpirationInSeconds = 30;

        public const string CachedStoryPrefixKey = "story";
        private const int CachedStoryExpirationInSeconds = 1800;

        private readonly ILogger<StoryProvider> _logger = logger;
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly IHackerNewsStoryHttpClient _storyHttpClient = storyHttpClient;

        public async Task<IEnumerable<int>> GetBestStoriesIdsAsync(int count)
        {
            // Search short-term cache for full list of best story ids
            var fullStoryIdList = await _memoryCache.GetOrCreateAsync(CachedStoryIdListKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(CacheStoryIdListExpirationInSeconds);
                return await _storyHttpClient.GetBestStoryIdListAsync();
            });

            if (fullStoryIdList is null)
            {
                _logger.LogError("Downloaded {FullStoryIdList} is null", nameof(fullStoryIdList));
                throw new InvalidOperationException($"Downloaded {nameof(fullStoryIdList)} is null");
            }

            var narrowedStoryIdList = fullStoryIdList.Take(count);
            return narrowedStoryIdList;
        }

        public async Task<StoryResponse?> GetStoryAsync(int storyId)
        {
            // Search long-term cache for story details
            var storyCacheKey = $"{CachedStoryPrefixKey}:{storyId}";
            var story = await _memoryCache.GetOrCreateAsync(storyCacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(CachedStoryExpirationInSeconds);
                return await _storyHttpClient.GetStoryAsync(storyId);
            });

            return story;
        }
    }
}
