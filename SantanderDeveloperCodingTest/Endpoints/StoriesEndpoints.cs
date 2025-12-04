using Microsoft.AspNetCore.Http.HttpResults;
using SantanderDeveloperCodingTest.Dtos;
using SantanderDeveloperCodingTest.Services;

namespace SantanderDeveloperCodingTest.Endpoints
{
    public static class StoriesEndpoints
    {
        public static async Task<Results<Ok<IEnumerable<StoryResponse>>,ProblemHttpResult>> GetBestStories(int n, IStoryService storyService)
        {
            if (n < 1)
            {
                return TypedResults.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid parameter",
                    detail: "Best stories count 'n' parameter should be bigger than 0.",
                    instance: "/bestStories");
            }
            var stories = await storyService.GetBestStoriesAsync(n);
            return TypedResults.Ok(stories);
        }
    }
}
