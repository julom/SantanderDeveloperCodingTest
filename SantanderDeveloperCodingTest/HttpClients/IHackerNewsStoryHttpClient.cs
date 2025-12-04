using SantanderDeveloperCodingTest.Dtos;

namespace SantanderDeveloperCodingTest.HttpClients
{
    public interface IHackerNewsStoryHttpClient
    {
        Task<IEnumerable<int>> GetBestStoryIdListAsync();
        Task<StoryResponse?> GetStoryAsync(int storyId);
    }
}