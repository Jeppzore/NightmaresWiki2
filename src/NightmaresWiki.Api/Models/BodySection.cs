namespace NightmaresWiki.Api.Models;

public sealed class BodySection
{
    public string Heading { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public List<string> Items { get; set; } = [];
}
