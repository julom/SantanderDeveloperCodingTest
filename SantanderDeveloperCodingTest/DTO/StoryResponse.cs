namespace SantanderDeveloperCodingTest.DTO
{
    public record StoryResponse(
        string? Title,
        string? Uri,
        string? PostedBy,
        DateTimeOffset? Time,
        int? Score,
        int? CommentCount
    );
}
