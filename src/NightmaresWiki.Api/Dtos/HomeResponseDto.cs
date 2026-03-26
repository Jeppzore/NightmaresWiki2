namespace NightmaresWiki.Api.Dtos;

public sealed class HomeResponseDto
{
    public string Title { get; init; } = string.Empty;

    public string Intro { get; init; } = string.Empty;

    public IReadOnlyList<HomeSectionDto> Sections { get; init; } = [];

    public IReadOnlyList<WikiEntrySummaryDto> Highlights { get; init; } = [];
}
