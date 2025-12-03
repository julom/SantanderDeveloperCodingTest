using Microsoft.AspNetCore.Mvc;
using Moq;
using SantanderDeveloperCodingTest.Dtos;
using SantanderDeveloperCodingTest.Test.TestHelpers;
using System.Net;
using System.Net.Http.Json;

namespace SantanderDeveloperCodingTest.Test.IntegrationTests
{
    public class StoriesEndpointTests
    {
        private TestWebApplicationFactory _factory;
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            _factory = new TestWebApplicationFactory();
            _client = _factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _factory.Dispose();
            _client.Dispose();
        }

        [Test]
        public async Task GetBestStories_ReturnsSortedByScore()
        {
            // arrange
            _factory.HackerNewsStoryFetcherMock
                .Setup(x => x.GetBestStoryIdListAsync())
                .ReturnsAsync(new[] { 10, 20, 30 });

            _factory.HackerNewsStoryFetcherMock
                .Setup(x => x.GetStoryAsync(10))
                .ReturnsAsync(RecordStubs.StoryResponse with { Title = "A", Score = 500 });
            _factory.HackerNewsStoryFetcherMock
                .Setup(x => x.GetStoryAsync(20))
                .ReturnsAsync(RecordStubs.StoryResponse with { Title = "B", Score = 300 });
            _factory.HackerNewsStoryFetcherMock
                .Setup(x => x.GetStoryAsync(30))
                .ReturnsAsync(RecordStubs.StoryResponse with { Title = "C", Score = 800 });

            // act
            var response = await _client.GetFromJsonAsync<List<StoryResponse>>("/bestStories?n=2");

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Count, Is.EqualTo(2));
            Assert.That(response[0].Score!.Value, Is.GreaterThanOrEqualTo(response[1].Score!.Value));
        }

        [Test]
        public async Task GetBestStories_WithInvalidCount_ReturnsProblemDetails()
        {
            // Arrange
            var url = "/bestStories?n=-1"; // invalid n parameter

            // Act
            var response = await _client.GetAsync(url);

            // Assert: status
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            // Assert: body as ProblemDetails
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            
            Assert.That(problem, Is.Not.Null);
            Assert.That(problem.Status, Is.EqualTo(400));
        }
    }
}
