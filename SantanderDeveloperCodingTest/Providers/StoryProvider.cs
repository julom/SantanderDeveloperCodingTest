using SantanderDeveloperCodingTest.Dtos;
using SantanderDeveloperCodingTest.HttpClients;

namespace SantanderDeveloperCodingTest.Providers
{
    public class StoryProvider(IHackerNewsStoryHttpClient storyHttpClient) : IStoryProvider
    {
        private readonly IHackerNewsStoryHttpClient _storyHttpClient = storyHttpClient;

        public async Task<IEnumerable<int>?> GetBestStoriesIdsAsync() => await _storyHttpClient.GetBestStoryIdListAsync();

        public async Task<StoryResponse?> GetStoryAsync(int storyId) => await _storyHttpClient.GetStoryAsync(storyId);
    }
}
