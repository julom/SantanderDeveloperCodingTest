using SantanderDeveloperCodingTest.Services;

namespace SantanderDeveloperCodingTest
{
    public static class EndpointHandlers
    {
        public static async Task<IResult> GetBestStories(int n, IStoryService storyService)
        {
            if (n < 1)
            {
                return Results.BadRequest("Best stories count 'n' parameter should be bigger than 0.");
            }
            var stories = await storyService.GetBestStoriesAsync(n);
            return Results.Ok(stories);
        }
    }
}
