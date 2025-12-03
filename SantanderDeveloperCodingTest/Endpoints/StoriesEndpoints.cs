using SantanderDeveloperCodingTest.Services;

namespace SantanderDeveloperCodingTest.Endpoints
{
    public static class StoriesEndpoints
    {
        public static async Task<IResult> GetBestStories(int n, IStoryService storyService)
        {
            if (n < 1)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid parameter",
                    detail: "Best stories count 'n' parameter should be bigger than 0.",
                    instance: "/bestStories");
            }
            var stories = await storyService.GetBestStoriesAsync(n);
            return Results.Ok(stories);
        }
    }
}
