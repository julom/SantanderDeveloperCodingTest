namespace SantanderDeveloperCodingTest.DTO
{
    public record StoryDto(
        int Id,
        string By,
        int Descendants,
        int Score,
        long Time,
        string Title,
        string Type,
        string Url
    );
}
