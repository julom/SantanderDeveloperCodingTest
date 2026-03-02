using SantanderDeveloperCodingTest.Dtos;

namespace SantanderDeveloperCodingTest.Providers
{
    public interface IStoryProvider
    {
        Task<IEnumerable<int>> GetBestStoriesIdsAsync(int count);
        Task<StoryResponse?> GetStoryAsync(int storyId);
    }
}