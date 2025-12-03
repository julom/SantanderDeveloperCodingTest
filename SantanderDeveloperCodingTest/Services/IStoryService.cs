using SantanderDeveloperCodingTest.DTO;

namespace SantanderDeveloperCodingTest.Services
{
    public interface IStoryService
    {
        Task<IEnumerable<StoryResponse>> GetBestStoriesAsync(int count);
    }
}