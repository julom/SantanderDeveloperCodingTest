using SantanderDeveloperCodingTest.Dtos;

namespace SantanderDeveloperCodingTest.Services
{
    public interface IHackerNewsStoryFetcher
    {
        Task<IEnumerable<int>> GetBestStoryIdListAsync();
        Task<StoryResponse?> GetStoryAsync(int storyId);
    }
}