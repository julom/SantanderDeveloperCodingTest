using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SantanderDeveloperCodingTest.Services;

namespace SantanderDeveloperCodingTest.Test.TestHelpers
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<IHackerNewsStoryFetcher> HackerNewsStoryFetcherMock { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // remove real client registration (if exists) and replace
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IHackerNewsStoryFetcher));
                if (descriptor != null) services.Remove(descriptor);

                services.AddSingleton(HackerNewsStoryFetcherMock.Object);
            });
        }
    }
}
