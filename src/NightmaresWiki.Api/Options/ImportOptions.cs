namespace NightmaresWiki.Api.Options;

public sealed class ImportOptions
{
    public const string SectionName = "Import";

    public string LegacyRoot { get; set; } = string.Empty;

    public string MediaOutputRelativePath { get; set; } = Path.Combine("wwwroot", "media", "imported");

    public bool ImportOnStartup { get; set; } = true;
}
