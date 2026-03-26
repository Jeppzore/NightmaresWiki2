namespace NightmaresWiki.Api.Dtos;

public sealed class HomeSectionDto
{
    public string Type { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public string Route { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public int Count { get; init; }

    public string? Image { get; init; }

    public string? StatusNote { get; init; }
}
