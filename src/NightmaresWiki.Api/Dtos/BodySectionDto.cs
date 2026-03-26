namespace NightmaresWiki.Api.Dtos;

public sealed class BodySectionDto
{
    public string Heading { get; init; } = string.Empty;

    public string Content { get; init; } = string.Empty;

    public IReadOnlyList<string> Items { get; init; } = [];
}
