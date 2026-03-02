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
            var selectedStoryIdList = (await _storyProvider.GetBestStoriesIdsAsync(count)).ToList(); //need to maintain the order

            if (selectedStoryIdList is null)
            {
                _logger.LogError("Resulted {SelectedStoryIdList} is null", nameof(selectedStoryIdList));
                throw new InvalidOperationException($"Resulted {nameof(selectedStoryIdList)} is null");
            }

            var tasks = selectedStoryIdList.Select(async storyId => await _storyProvider.GetStoryAsync(storyId));
            var stories = await Task.WhenAll(tasks);

            return stories.OfType<StoryResponse>();
        }
    }
}
