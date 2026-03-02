using SantanderDeveloperCodingTest.Dtos;
using SantanderDeveloperCodingTest.Providers;

namespace SantanderDeveloperCodingTest.Services
{
    public class StoryService(
        ILogger<StoryService> logger,
        IStoryProvider storyProvider) : IStoryService
    {
        private readonly ILogger<StoryService> _logger = logger;
        private readonly IStoryProvider _storyProvider = storyProvider;

        public async Task<IEnumerable<StoryResponse>> GetBestStoriesAsync(int count)
        {
            var fullStoryIdList = await _storyProvider.GetBestStoriesIdsAsync();

            if (fullStoryIdList is null)
            {
                _logger.LogError("Downloaded {FullStoryIdList} is null", nameof(fullStoryIdList));
                throw new InvalidOperationException($"Downloaded {nameof(fullStoryIdList)} is null");
            }

            var selectedStoryIdList = fullStoryIdList.Take(count).ToList(); // need to maintain the order

            var tasks = selectedStoryIdList.Select(async storyId => await _storyProvider.GetStoryAsync(storyId));
            var stories = await Task.WhenAll(tasks);

            return stories.OfType<StoryResponse>();
        }
    }
}
