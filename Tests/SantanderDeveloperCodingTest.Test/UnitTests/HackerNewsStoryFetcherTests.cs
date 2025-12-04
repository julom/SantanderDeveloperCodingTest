using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Legacy;
using SantanderDeveloperCodingTest.Dtos;
using SantanderDeveloperCodingTest.HttpClients;
using SantanderDeveloperCodingTest.Test.TestHelpers;
using System.Net;
using System.Text;
using System.Text.Json;

namespace SantanderDeveloperCodingTest.Test.UnitTests
{
    public class HackerNewsStoryFetcherTests
    {
        const string BaseAddress = "https://hacker-news.firebaseio.com/v0/";

        private Mock<ILogger<HackerNewsStoryHttpClient>> _loggerMock;
        private Mock<IMapper> _mapperMock;

        [SetUp]
        public void SetUp()
        {
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<HackerNewsStoryHttpClient>>();
        }

        private HttpClient CreateHttpClient(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responder)
        {
            var handler = new FakeHttpMessageHandler(responder);
            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseAddress)
            };
            return client;
        }

        [Test]
        public async Task GetBestStoryIdListAsync_ReturnsStoryIds_WhenSuccessful()
        {
            // Arrange
            var ids = new[] { 10, 20, 30 };
            var json = JsonSerializer.Serialize(ids);
            var httpClient = CreateHttpClient((req, ct) =>
            {
                if (req.RequestUri?.AbsolutePath.EndsWith("beststories.json") == true)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json")
                    };
                    return Task.FromResult(resp);
                }
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
            });

            var fetcher = new HackerNewsStoryHttpClient(_loggerMock.Object, httpClient, _mapperMock.Object);

            // Act
            var result = await fetcher.GetBestStoryIdListAsync();

            // Assert
            CollectionAssert.AreEqual(ids, result);
        }

        [Test]
        public async Task GetBestStoryIdListAsync_ReturnsEmptyArray_WhenJsonIsNull()
        {
            // Arrange
            var httpClient = CreateHttpClient((req, ct) =>
            {
                if (req.RequestUri?.AbsolutePath.EndsWith("beststories.json") == true)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("null", Encoding.UTF8, "application/json")
                    };
                    return Task.FromResult(resp);
                }
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
            });

            var fetcher = new HackerNewsStoryHttpClient(_loggerMock.Object, httpClient, _mapperMock.Object);

            // Act
            var result = await fetcher.GetBestStoryIdListAsync();

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetBestStoryIdListAsync_LogsAndRethrows_WhenHandlerThrows()
        {
            // Arrange: handler throws to simulate network failure
            var httpClient = CreateHttpClient((req, ct) =>
            {
                throw new HttpRequestException("Simulated network failure");
            });

            var fetcher = new HackerNewsStoryHttpClient(_loggerMock.Object, httpClient, _mapperMock.Object);

            // Act & Assert
            Assert.ThrowsAsync<HttpRequestException>(async () => await fetcher.GetBestStoryIdListAsync());
        }

        [Test]
        public async Task GetStoryAsync_ReturnsMappedStory_WhenSuccessful()
        {
            // Arrange
            var storyId = 123;
            var dto = RecordStubs.StoryDto with { Id = storyId };
            var dtoJson = JsonSerializer.Serialize(dto);
            var httpClient = CreateHttpClient((req, ct) =>
            {
                if (req.RequestUri?.AbsolutePath.EndsWith($"item/{storyId}.json") == true)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(dtoJson, Encoding.UTF8, "application/json")
                    };
                    return Task.FromResult(resp);
                }
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
            });

            var expectedResponse = new StoryResponse(dto.Title, dto.Url, dto.By, DateTimeOffset.FromUnixTimeSeconds(dto.Time), dto.Score, dto.Descendants);

            _mapperMock
                .Setup(m => m.Map<StoryResponse>(It.Is<StoryDto>(d => d != null && d.Id == storyId)))
                .Returns(expectedResponse);

            var fetcher = new HackerNewsStoryHttpClient(_loggerMock.Object, httpClient, _mapperMock.Object);

            // Act
            var result = await fetcher.GetStoryAsync(storyId);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(expectedResponse, result);
            _mapperMock.Verify(m => m.Map<StoryResponse>(It.IsAny<StoryDto>()), Times.Once);
        }

        [Test]
        public async Task GetStoryAsync_LogsAndReturnsNull_WhenHandlerThrows()
        {
            // Arrange
            var storyId = 999;
            var httpClient = CreateHttpClient((req, ct) =>
            {
                throw new HttpRequestException("Simulated fetch error");
            });

            var fetcher = new HackerNewsStoryHttpClient(_loggerMock.Object, httpClient, _mapperMock.Object);

            // Act
            var result = await fetcher.GetStoryAsync(storyId);

            // Assert
            ClassicAssert.IsNull(result);
        }
    }
}
