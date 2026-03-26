namespace NightmaresWiki.Api.Dtos;

public sealed class EntryRelationshipDto
{
    public string Type { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public string Slug { get; init; } = string.Empty;

    public string? Image { get; init; }
}
