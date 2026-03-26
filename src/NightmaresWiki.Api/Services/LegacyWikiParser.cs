using System.Net;
using System.Text.RegularExpressions;
using NightmaresWiki.Api.Models;

namespace NightmaresWiki.Api.Services;

public sealed partial class LegacyWikiParser
{
    public WikiEntry ParseEnemy(string path, string legacyRoot) => ParseFile(path, legacyRoot, "enemy", true);

    public WikiEntry ParseItem(string path, string legacyRoot) => ParseFile(path, legacyRoot, "item", true);

    public WikiEntry ParseNpcSection(string path, string legacyRoot)
    {
        var entry = ParseFile(path, legacyRoot, "npc", false);
        entry.Slug = "npcs";
        entry.Title = "NPC";
        entry.Summary = string.IsNullOrWhiteSpace(entry.Summary)
            ? "Nightmares does not have dedicated NPC articles in the legacy source yet."
            : entry.Summary;

        if (entry.BodySections.Count == 0)
        {
            entry.BodySections.Add(new BodySection
            {
                Heading = "Status",
                Content = "The legacy Nightmares wiki only provides an NPC landing page right now. This section is intentionally sparse until dedicated NPC articles exist."
            });
        }

        return entry;
    }

    private WikiEntry ParseFile(string path, string legacyRoot, string type, bool isDetailPage)
    {
        var html = File.ReadAllText(path);
        var mainContent = ExtractMainContent(html);
        var title = ExtractTitle(mainContent, html, path);
        var image = ResolveImagePath(ExtractPrimaryImage(mainContent, html), path, legacyRoot);
        var sections = ExtractSections(mainContent);
        var stats = ExtractStats(mainContent);
        var relationships = ExtractRelationships(mainContent, type);
        var summary = ExtractSummary(mainContent, title, type);
        var taxonomy = BuildTaxonomy(path, legacyRoot, type);

        return new WikiEntry
        {
            Type = type,
            Slug = Slugify(title),
            Title = title,
            Summary = summary,
            BodySections = sections,
            Stats = stats,
            Taxonomy = taxonomy,
            TaxonomyKeys = taxonomy.Select(Slugify).ToList(),
            Relationships = relationships,
            Image = image,
            SourcePath = path,
            IsDetailPage = isDetailPage,
            ImportedAtUtc = DateTime.UtcNow
        };
    }

    private static List<string> BuildTaxonomy(string path, string legacyRoot, string type)
    {
        if (type == "enemy")
        {
            return ["Enemies"];
        }

        if (type == "npc")
        {
            return ["NPC"];
        }

        var itemsRoot = Path.Combine(legacyRoot, "items");
        var relativeDirectory = Path.GetRelativePath(itemsRoot, Path.GetDirectoryName(path)!);
        if (relativeDirectory is "." or "")
        {
            return ["Items"];
        }

        var separators = new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
        var segments = relativeDirectory
            .Split(separators, StringSplitOptions.RemoveEmptyEntries)
            .Select(ToLabel)
            .ToList();

        return ["Items", .. segments];
    }

    private static string ExtractMainContent(string html)
    {
        var match = MainContentRegex().Match(html);
        return match.Success ? match.Groups["content"].Value : html;
    }

    private static string ExtractTitle(string mainContent, string html, string path)
    {
        var itemNameMatch = ItemNameRegex().Match(mainContent);
        if (itemNameMatch.Success)
        {
            return CleanText(ImageTagRegex().Replace(itemNameMatch.Groups["inner"].Value, string.Empty));
        }

        var firstHeadingMatch = H1Regex().Match(mainContent);
        if (firstHeadingMatch.Success)
        {
            return CleanText(firstHeadingMatch.Groups["inner"].Value);
        }

        var titleMatch = TitleRegex().Match(html);
        if (titleMatch.Success)
        {
            return CleanText(titleMatch.Groups["title"].Value.Replace("| Nightmares", string.Empty));
        }

        return ToLabel(Path.GetFileNameWithoutExtension(path));
    }

    private static string? ExtractPrimaryImage(string mainContent, string html)
    {
        var itemNameMatch = ItemNameRegex().Match(mainContent);
        if (itemNameMatch.Success)
        {
            var imageMatch = ImageSourceRegex().Match(itemNameMatch.Groups["inner"].Value);
            if (imageMatch.Success)
            {
                return imageMatch.Groups["src"].Value;
            }
        }

        var bodyImageMatch = ImageSourceRegex().Match(mainContent);
        if (bodyImageMatch.Success)
        {
            return bodyImageMatch.Groups["src"].Value;
        }

        var pageImageMatch = ImageSourceRegex().Match(html);
        return pageImageMatch.Success ? pageImageMatch.Groups["src"].Value : null;
    }

    private static string ExtractSummary(string mainContent, string title, string type)
    {
        var headingMatch = ItemNameRegex().Match(mainContent);
        if (!headingMatch.Success)
        {
            headingMatch = H1Regex().Match(mainContent);
        }

        if (!headingMatch.Success)
        {
            return DefaultSparseSummary(title, type);
        }

        var remaining = mainContent[(headingMatch.Index + headingMatch.Length)..];
        var nextHeadingIndex = remaining.IndexOf("<h2", StringComparison.OrdinalIgnoreCase);
        var rawSummary = nextHeadingIndex >= 0 ? remaining[..nextHeadingIndex] : remaining;
        var cleaned = CleanText(rawSummary);

        if (!string.IsNullOrWhiteSpace(cleaned))
        {
            return cleaned;
        }

        return DefaultSparseSummary(title, type);
    }

    private static string DefaultSparseSummary(string title, string type) =>
        type switch
        {
            "item" => $"{title} exists in the legacy Nightmares repository, but its detailed article has not been authored yet.",
            "enemy" => $"{title} exists in the legacy Nightmares repository, but its detailed article has not been authored yet.",
            _ => title
        };

    private static List<BodySection> ExtractSections(string mainContent)
    {
        var sections = new List<BodySection>();

        foreach (Match match in SectionRegex().Matches(mainContent))
        {
            var heading = CleanText(match.Groups["heading"].Value);
            var body = match.Groups["body"].Value;
            var items = ExtractSectionItems(body);
            var content = CleanText(ListElementRegex().Replace(body, string.Empty));

            if (string.IsNullOrWhiteSpace(content) && items.Count == 0)
            {
                continue;
            }

            sections.Add(new BodySection
            {
                Heading = heading,
                Content = content,
                Items = items
            });
        }

        return sections;
    }

    private static List<WikiStat> ExtractStats(string mainContent)
    {
        var stats = new List<WikiStat>();
        foreach (Match statsContainer in StatsListRegex().Matches(mainContent))
        {
            foreach (Match item in ListItemRegex().Matches(statsContainer.Groups["items"].Value))
            {
                var labelMatch = BoldLabelRegex().Match(item.Groups["item"].Value);
                if (!labelMatch.Success)
                {
                    continue;
                }

                var remainder = item.Groups["item"].Value[(labelMatch.Index + labelMatch.Length)..];
                var value = CleanText(remainder);

                stats.Add(new WikiStat
                {
                    Label = CleanText(labelMatch.Groups["label"].Value),
                    Value = value
                });
            }
        }

        return stats;
    }

    private static List<EntryRelationship> ExtractRelationships(string mainContent, string entryType)
    {
        var relationships = new List<EntryRelationship>();

        foreach (Match sectionMatch in SectionRegex().Matches(mainContent))
        {
            var heading = CleanText(sectionMatch.Groups["heading"].Value);
            foreach (Match linkMatch in AnchorRegex().Matches(sectionMatch.Groups["body"].Value))
            {
                var href = linkMatch.Groups["href"].Value;
                var linkText = CleanText(linkMatch.Groups["text"].Value);
                var slug = Slugify(string.IsNullOrWhiteSpace(linkText) ? Path.GetFileNameWithoutExtension(href) : linkText);
                if (string.IsNullOrWhiteSpace(slug))
                {
                    continue;
                }

                relationships.Add(new EntryRelationship
                {
                    Type = GetRelationshipType(entryType, heading),
                    Title = linkText,
                    Slug = slug
                });
            }
        }

        return relationships
            .GroupBy(relationship => $"{relationship.Type}|{relationship.Slug}", StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .ToList();
    }

    private static List<string> ExtractSectionItems(string body)
    {
        var items = new List<string>();

        foreach (Match listItem in ListItemRegex().Matches(body))
        {
            var itemText = CleanText(listItem.Groups["item"].Value);
            if (!string.IsNullOrWhiteSpace(itemText))
            {
                items.Add(itemText);
            }
        }

        foreach (Match boldItem in BoldStandaloneRegex().Matches(body))
        {
            var itemText = CleanText(boldItem.Groups["text"].Value);
            if (!string.IsNullOrWhiteSpace(itemText) && !items.Contains(itemText, StringComparer.OrdinalIgnoreCase))
            {
                items.Add(itemText);
            }
        }

        return items;
    }

    private static string GetRelationshipType(string entryType, string heading)
    {
        if (entryType == "item" && heading.Contains("Dropped by", StringComparison.OrdinalIgnoreCase))
        {
            return "droppedBy";
        }

        return "related";
    }

    private static string ResolveImagePath(string? rawPath, string sourcePath, string legacyRoot)
    {
        if (!string.IsNullOrWhiteSpace(rawPath))
        {
            return NormalizeImagePath(rawPath);
        }

        var imageDirectory = Path.Combine(legacyRoot, "images");
        var sourceBaseName = Path.GetFileNameWithoutExtension(sourcePath);
        var candidates = Directory.EnumerateFiles(imageDirectory, "*", SearchOption.TopDirectoryOnly)
            .Select(file => new
            {
                File = file,
                BaseName = Path.GetFileNameWithoutExtension(file)
            })
            .ToList();

        var directMatch = candidates.FirstOrDefault(candidate =>
            string.Equals(candidate.BaseName, sourceBaseName, StringComparison.OrdinalIgnoreCase));

        if (directMatch is not null)
        {
            return NormalizeImagePath(directMatch.File);
        }

        var sluggedSource = Slugify(sourceBaseName);
        var looseMatch = candidates.FirstOrDefault(candidate =>
            string.Equals(Slugify(candidate.BaseName), sluggedSource, StringComparison.OrdinalIgnoreCase));

        return looseMatch is not null ? NormalizeImagePath(looseMatch.File) : string.Empty;
    }

    private static string NormalizeImagePath(string? rawPath)
    {
        if (string.IsNullOrWhiteSpace(rawPath))
        {
            return string.Empty;
        }

        var normalized = rawPath.Trim().TrimStart('\\', '/').Replace('\\', '/');
        var fileName = Path.GetFileName(normalized);
        return $"/media/imported/{fileName}";
    }

    private static string CleanText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var withoutTags = TagRegex().Replace(text, " ");
        var decoded = WebUtility.HtmlDecode(withoutTags)
            .Replace("â€”", "-")
            .Replace("â†‘", "↑");

        return WhitespaceRegex().Replace(decoded, " ").Trim();
    }

    internal static string Slugify(string value)
    {
        var cleaned = value.Replace(".html", string.Empty, StringComparison.OrdinalIgnoreCase);
        cleaned = CamelCaseBoundaryRegex().Replace(cleaned, "$1-$2");
        cleaned = cleaned.Replace("_", "-");
        cleaned = NonSlugCharacterRegex().Replace(cleaned.ToLowerInvariant(), "-");
        cleaned = MultiDashRegex().Replace(cleaned, "-");
        return cleaned.Trim('-');
    }

    private static string ToLabel(string value)
    {
        var withSpaces = CamelCaseBoundaryRegex().Replace(value, "$1 $2").Replace("-", " ");
        return string.Join(" ", withSpaces
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(segment => char.ToUpperInvariant(segment[0]) + segment[1..].ToLowerInvariant()));
    }

    [GeneratedRegex("<main[^>]*class=\"content\"[^>]*>(?<content>.*?)</main>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex MainContentRegex();

    [GeneratedRegex("<h1[^>]*class=\"item-name\"[^>]*>(?<inner>.*?)</h1>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex ItemNameRegex();

    [GeneratedRegex("<h1[^>]*>(?<inner>.*?)</h1>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex H1Regex();

    [GeneratedRegex("<title>(?<title>.*?)</title>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex TitleRegex();

    [GeneratedRegex("<img[^>]*src=\"(?<src>[^\"]+)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex ImageSourceRegex();

    [GeneratedRegex("<img[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex ImageTagRegex();

    [GeneratedRegex("<h2[^>]*>(?<heading>.*?)</h2>(?<body>.*?)(?=<h2|<button|</main>)", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex SectionRegex();

    [GeneratedRegex("<ul[^>]*class=\"[^\"]*stats[^\"]*\"[^>]*>(?<items>.*?)</ul>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex StatsListRegex();

    [GeneratedRegex("<li[^>]*>(?<item>.*?)</li>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex ListItemRegex();

    [GeneratedRegex("<b>(?<label>[^<:]+):</b>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex BoldLabelRegex();

    [GeneratedRegex("<a[^>]*href=\"(?<href>[^\"]+)\"[^>]*>(?<text>.*?)</a>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex AnchorRegex();

    [GeneratedRegex("<b>(?<text>.*?)</b>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex BoldStandaloneRegex();

    [GeneratedRegex("<(ul|ol|li|div|p|br|hr)[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex ListElementRegex();

    [GeneratedRegex("<[^>]+>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex TagRegex();

    [GeneratedRegex("\\s+", RegexOptions.Singleline)]
    private static partial Regex WhitespaceRegex();

    [GeneratedRegex("([a-z0-9])([A-Z])", RegexOptions.Singleline)]
    private static partial Regex CamelCaseBoundaryRegex();

    [GeneratedRegex("[^a-z0-9]+", RegexOptions.Singleline)]
    private static partial Regex NonSlugCharacterRegex();

    [GeneratedRegex("-{2,}", RegexOptions.Singleline)]
    private static partial Regex MultiDashRegex();
}
