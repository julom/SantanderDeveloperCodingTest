using Microsoft.Extensions.Caching.Memory;
using SantanderDeveloperCodingTest.Dtos;

namespace SantanderDeveloperCodingTest.Providers
{
    public class CachedStoryProvider(
        IMemoryCache memoryCache,
        IStoryProvider inner) : IStoryProvider
    {
        public const string CachedStoryIdListKey = "full_list_of_story_ids";
        private const int CacheStoryIdListExpirationInSeconds = 30;

        public const string CachedStoryPrefixKey = "story";
        private const int CachedStoryExpirationInSeconds = 1800;

        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly IStoryProvider _inner = inner;

        public async Task<IEnumerable<int>?> GetBestStoriesIdsAsync()
        {
            // Search short-term cache for full list of best story ids
            var fullStoryIdList = await _memoryCache.GetOrCreateAsync(CachedStoryIdListKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(CacheStoryIdListExpirationInSeconds);
                return await _inner.GetBestStoriesIdsAsync();
            });

            return fullStoryIdList;
        }

        public async Task<StoryResponse?> GetStoryAsync(int storyId)
        {
            // Search long-term cache for story details
            var storyCacheKey = $"{CachedStoryPrefixKey}:{storyId}";
            var story = await _memoryCache.GetOrCreateAsync(storyCacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(CachedStoryExpirationInSeconds);
                return await _inner.GetStoryAsync(storyId);
            });

            return story;
        }
    }
}
