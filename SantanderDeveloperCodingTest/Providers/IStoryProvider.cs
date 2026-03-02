using SantanderDeveloperCodingTest.Dtos;

namespace SantanderDeveloperCodingTest.Providers
{
    public interface IStoryProvider
    {
        Task<IEnumerable<int>?> GetBestStoriesIdsAsync();
        Task<StoryResponse?> GetStoryAsync(int storyId);
    }
}