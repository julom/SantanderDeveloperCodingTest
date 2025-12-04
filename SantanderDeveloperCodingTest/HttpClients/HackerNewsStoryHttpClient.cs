using AutoMapper;
using SantanderDeveloperCodingTest.Dtos;

namespace SantanderDeveloperCodingTest.HttpClients
{
    public class HackerNewsStoryHttpClient(
        ILogger<HackerNewsStoryHttpClient> logger,
        HttpClient httpClient,
        IMapper mapper) : IHackerNewsStoryHttpClient
    {
        private readonly ILogger<HackerNewsStoryHttpClient> _logger = logger;
        private readonly HttpClient _httpClient = httpClient;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<int>> GetBestStoryIdListAsync()
        {
            try
            {
                var storyIdList = await _httpClient.GetFromJsonAsync<IEnumerable<int>>("beststories.json");
                return storyIdList ?? Array.Empty<int>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not fetch story id list");
                throw;
            }
        }

        public async Task<StoryResponse?> GetStoryAsync(int storyId)
        {
            try
            {
                var path = $"item/{storyId}.json";
                var story = await _httpClient.GetFromJsonAsync<StoryDto>(path);
                var mappedStory = _mapper.Map<StoryResponse>(story);
                return mappedStory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Could not fetch {storyId}", storyId);
                return null;
            }
        }
    }
}
