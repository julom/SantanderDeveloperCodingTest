using Microsoft.AspNetCore.RateLimiting;
using Polly;
using SantanderDeveloperCodingTest.Dtos;
using SantanderDeveloperCodingTest.Endpoints;
using SantanderDeveloperCodingTest.Services;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();
builder.Services.AddHealthChecks();
builder.Services.AddProblemDetails();
builder.Services.AddHttpClient<IHackerNewsStoryFetcher, HackerNewsStoryFetcher>(client =>
{
    client.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
    client.Timeout = TimeSpan.FromSeconds(5);
})
.AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(3, retry => TimeSpan.FromSeconds(5)));

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromSeconds(10);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;
    });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .WithMethods("HEAD", "GET")
              .WithOrigins("https://example.com");
    });
});

builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder =>
        builder.Expire(TimeSpan.FromSeconds(3)));
});

builder.Services.AddTransient<IStoryService, StoryService>();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.CreateMap<StoryDto, StoryResponse>()
        .ForCtorParam(nameof(StoryResponse.CommentCount), opt => opt.MapFrom(src => src.Descendants))
        .ForCtorParam(nameof(StoryResponse.Uri), opt => opt.MapFrom(src => src.Url))
        .ForCtorParam(nameof(StoryResponse.PostedBy), opt => opt.MapFrom(src => src.By))
        .ForCtorParam(nameof(StoryResponse.Time), opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.Time)));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseCors();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseOutputCache();
app.UseRateLimiter();
app.UseHttpsRedirection();

app.UseHealthChecks("/health");

app.MapGet("/bestStories", EndpointHandlers.GetBestStories)
.WithName("GetBestStories")
.CacheOutput()
.RequireRateLimiting("fixed");

app.Run();

public partial class Program { } //required by integration tests