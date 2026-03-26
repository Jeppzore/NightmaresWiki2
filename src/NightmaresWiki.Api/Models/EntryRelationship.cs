namespace NightmaresWiki.Api.Models;

public sealed class EntryRelationship
{
    public string Type { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? Image { get; set; }
}
