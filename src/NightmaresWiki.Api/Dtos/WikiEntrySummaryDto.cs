namespace NightmaresWiki.Api.Dtos;

public class WikiEntrySummaryDto
{
    public string Type { get; init; } = string.Empty;

    public string Slug { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public string Summary { get; init; } = string.Empty;

    public IReadOnlyList<string> Taxonomy { get; init; } = [];

    public string? Image { get; init; }

    public bool IsDetailPage { get; init; }
}
