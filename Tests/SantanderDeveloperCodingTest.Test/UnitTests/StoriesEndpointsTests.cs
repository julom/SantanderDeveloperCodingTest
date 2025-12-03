using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework.Legacy;
using SantanderDeveloperCodingTest.Dtos;
using SantanderDeveloperCodingTest.Endpoints;
using SantanderDeveloperCodingTest.Services;
using SantanderDeveloperCodingTest.Test.TestHelpers;

namespace SantanderDeveloperCodingTest.Test.UnitTests
{
    public class StoriesEndpointsTests
    {
        private Mock<IStoryService> _storyServiceMock;

        [SetUp]
        public void SetUp()
        {
            _storyServiceMock = new Mock<IStoryService>();
        }

        [Test]
        public async Task GetBestStories_CountLessThanOne_ReturnsBadRequest()
        {
            // Act
            var result = await StoriesEndpoints.GetBestStories(0, _storyServiceMock.Object);

            // Assert
            ClassicAssert.IsInstanceOf<ProblemHttpResult>(result, "Expected a BadRequest when count < 1.");
            _storyServiceMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task GetBestStories_ValidCount_ReturnsOkWithStories_AndCallsService()
        {
            // Arrange
            var stories = new List<StoryResponse>
            {
                RecordStubs.StoryResponse with {Title = "title1"},
                RecordStubs.StoryResponse with {Title = "title2"}
            };

            _storyServiceMock.Setup(s => s.GetBestStoriesAsync(2)).ReturnsAsync(stories);

            // Act
            var result = await StoriesEndpoints.GetBestStories(2, _storyServiceMock.Object);

            // Assert
            ClassicAssert.IsInstanceOf<Ok<IEnumerable<StoryResponse>>>(result, "Expected an Ok result for valid count.");
            var okResult = (Ok<IEnumerable<StoryResponse>>)result;
            ClassicAssert.IsNotNull(okResult.Value);
            CollectionAssert.AreEqual(stories, okResult.Value);
            _storyServiceMock.Verify(s => s.GetBestStoriesAsync(2), Times.Once);
            _storyServiceMock.VerifyNoOtherCalls();
        }
    }
}
