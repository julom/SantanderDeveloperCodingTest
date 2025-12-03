using SantanderDeveloperCodingTest.Dtos;

namespace SantanderDeveloperCodingTest.Test.TestHelpers
{
    public static class RecordStubs
    {
        public static StoryDto StoryDto { get; set; } = new (123, "By", 1, 10, 1764678514, "Title", "Story", "url");

        public static StoryResponse StoryResponse { get; set; } = new("Title", "url", "By", DateTimeOffset.FromUnixTimeSeconds(1764678514), 10, 1);
    }
}
