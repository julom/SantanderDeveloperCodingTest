using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SantanderDeveloperCodingTest.HttpClients;

namespace SantanderDeveloperCodingTest.Test.TestHelpers
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<IHackerNewsStoryHttpClient> HackerNewsStoryFetcherMock { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // remove real client registration (if exists) and replace
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IHackerNewsStoryHttpClient));
                if (descriptor != null) services.Remove(descriptor);

                services.AddSingleton(HackerNewsStoryFetcherMock.Object);
            });
        }
    }
}
