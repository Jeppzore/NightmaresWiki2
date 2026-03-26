using Microsoft.Extensions.Options;
using NightmaresWiki.Api.Models;
using NightmaresWiki.Api.Options;
using NightmaresWiki.Api.Repositories;

namespace NightmaresWiki.Api.Services;

public sealed class LegacyWikiImportService(
    LegacyWikiParser parser,
    IWikiEntryRepository repository,
    IWebHostEnvironment environment,
    IOptions<ImportOptions> importOptions) : IContentImportService
{
    public async Task<ImportSummary> ImportAsync(CancellationToken cancellationToken)
    {
        var options = importOptions.Value;
        if (string.IsNullOrWhiteSpace(options.LegacyRoot) || !Directory.Exists(options.LegacyRoot))
        {
            throw new DirectoryNotFoundException($"Legacy NightmaresWiki root not found: {options.LegacyRoot}");
        }

        CopyImages(options.LegacyRoot, environment.ContentRootPath, options.MediaOutputRelativePath);

        var enemyPaths = Directory
            .EnumerateFiles(Path.Combine(options.LegacyRoot, "creatures"), "*.html", SearchOption.TopDirectoryOnly)
            .Where(path => !Path.GetFileName(path).Contains("home", StringComparison.OrdinalIgnoreCase))
            .OrderBy(path => path)
            .ToList();

        var itemPaths = Directory
            .EnumerateFiles(Path.Combine(options.LegacyRoot, "items"), "*.html", SearchOption.AllDirectories)
            .Where(path =>
                !Path.GetFileName(path).Contains("home", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(Path.GetFileName(path), "itemsHome.html", StringComparison.OrdinalIgnoreCase))
            .OrderBy(path => path)
            .ToList();

        var enemyEntries = enemyPaths.Select(path => parser.ParseEnemy(path, options.LegacyRoot)).ToList();
        var itemEntries = itemPaths.Select(path => parser.ParseItem(path, options.LegacyRoot)).ToList();
        var npcMetadata = parser.ParseNpcSection(Path.Combine(options.LegacyRoot, "npc.html"), options.LegacyRoot);

        EnrichReverseRelationships(enemyEntries, itemEntries);

        var allEntries = enemyEntries
            .Concat(itemEntries)
            .Append(npcMetadata)
            .ToList();

        await repository.ReplaceAllAsync(allEntries, cancellationToken);

        return new ImportSummary
        {
            Enemies = enemyEntries.Count,
            Items = itemEntries.Count,
            NpcMetadataEntries = 1
        };
    }

    private static void CopyImages(string legacyRoot, string contentRootPath, string mediaOutputRelativePath)
    {
        var sourceDirectory = Path.Combine(legacyRoot, "images");
        var targetDirectory = Path.Combine(contentRootPath, mediaOutputRelativePath);

        Directory.CreateDirectory(targetDirectory);

        foreach (var file in Directory.EnumerateFiles(sourceDirectory, "*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(sourceDirectory, file);
            var destination = Path.Combine(targetDirectory, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            File.Copy(file, destination, overwrite: true);
        }
    }

    private static void EnrichReverseRelationships(List<WikiEntry> enemyEntries, List<WikiEntry> itemEntries)
    {
        var enemiesBySlug = enemyEntries.ToDictionary(entry => entry.Slug, StringComparer.OrdinalIgnoreCase);

        foreach (var item in itemEntries)
        {
            foreach (var relationship in item.Relationships.Where(relationship => relationship.Type == "droppedBy"))
            {
                if (!enemiesBySlug.TryGetValue(relationship.Slug, out var enemy))
                {
                    continue;
                }

                var alreadyAdded = enemy.Relationships.Any(existing =>
                    existing.Type == "drops" &&
                    string.Equals(existing.Slug, item.Slug, StringComparison.OrdinalIgnoreCase));

                if (!alreadyAdded)
                {
                    enemy.Relationships.Add(new EntryRelationship
                    {
                        Type = "drops",
                        Title = item.Title,
                        Slug = item.Slug,
                        Image = item.Image
                    });
                }
            }
        }
    }
}
