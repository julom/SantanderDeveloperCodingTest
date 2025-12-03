using Microsoft.Extensions.Caching.Memory;
using SantanderDeveloperCodingTest.DTO;

namespace SantanderDeveloperCodingTest.Services
{
    public class StoryService(
        ILogger<StoryService> logger,
        IMemoryCache memoryCache,
        IHackerNewsStoryFetcher newsFetcher) : IStoryService
    {
        public const string CachedStoryIdListKey = "full_list_of_story_ids";
        private const int CacheStoryIdListExpirationInSeconds = 30;

        public const string CachedStoryPrefixKey = "story";
        private const int CachedStoryExpirationInSeconds = 1800;

        private readonly ILogger<StoryService> _logger = logger;
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly IHackerNewsStoryFetcher _newsFetcher = newsFetcher;

        public async Task<IEnumerable<StoryResponse>> GetBestStoriesAsync(int count)
        {
            // Search short-term cache for full list of best story ids
            var fullStoryIdList = await _memoryCache.GetOrCreateAsync(CachedStoryIdListKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(CacheStoryIdListExpirationInSeconds);
                return await _newsFetcher.GetBestStoryIdListAsync();
            });

            if (fullStoryIdList is null)
            {
                _logger.LogError("Downloaded {FullStoryIdList} is null", nameof(fullStoryIdList));
                throw new InvalidOperationException($"Downloaded {nameof(fullStoryIdList)} is null");
            }

            var narrowedStoryIdList = fullStoryIdList.Take(count);

            await Parallel.ForEachAsync(narrowedStoryIdList, async (storyId, ct) =>
            {
                // Search long-term cache for story details
                var storyCacheKey = $"{CachedStoryPrefixKey}:{storyId}";
                await _memoryCache.GetOrCreateAsync(storyCacheKey, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(CachedStoryExpirationInSeconds);
                    return await _newsFetcher.GetStoryAsync(storyId);
                });
            });

            // Get ordered list of stories from cache which should have all required stories now
            var stories = narrowedStoryIdList
                .Select(storyId =>
                {
                    var storyCacheKey = $"{CachedStoryPrefixKey}:{storyId}";
                    var story = _memoryCache.Get<StoryResponse?>(storyCacheKey);
                    return story;
                })
                .OfType<StoryResponse>()
                .ToList();

            return stories;
        }
    }
}
