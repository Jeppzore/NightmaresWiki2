namespace NightmaresWiki.Api.Dtos;

public sealed class WikiEntryDetailDto : WikiEntrySummaryDto
{
    public IReadOnlyList<BodySectionDto> BodySections { get; init; } = [];

    public IReadOnlyList<WikiStatDto> Stats { get; init; } = [];

    public IReadOnlyList<EntryRelationshipDto> Relationships { get; init; } = [];

    public string SourcePath { get; init; } = string.Empty;
}
