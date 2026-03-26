using NightmaresWiki.Api.Services;

namespace NightmaresWiki.Api.Tests.Parsing;

public sealed class LegacyWikiParserTests
{
    private readonly LegacyWikiParser parser = new();
    private const string LegacyRoot = @"C:\Users\jesal\source\repos\NightmaresWiki";

    [Fact]
    public void ParseEnemy_ExtractsTitleStatsAndSections()
    {
        var path = Path.Combine(LegacyRoot, "creatures", "greenslime.html");

        var entry = parser.ParseEnemy(path, LegacyRoot);

        Assert.Equal("enemy", entry.Type);
        Assert.Equal("green-slime", entry.Slug);
        Assert.Equal("Green Slime", entry.Title);
        Assert.Contains(entry.Stats, stat => stat.Label == "Health" && stat.Value == "5");
        Assert.Contains(entry.BodySections, section => section.Heading == "About");
        Assert.Equal("/media/imported/greenslimegif.gif", entry.Image);
    }

    [Fact]
    public void ParseItem_ExtractsTaxonomyImageAndRelationships()
    {
        var path = Path.Combine(LegacyRoot, "items", "Weapons", "melee", "bronzeSword.html");

        var entry = parser.ParseItem(path, LegacyRoot);

        Assert.Equal("item", entry.Type);
        Assert.Equal("bronze-sword", entry.Slug);
        Assert.Equal(new[] { "Items", "Weapons", "Melee" }, entry.Taxonomy);
        Assert.Contains(entry.Relationships, relationship => relationship.Type == "droppedBy" && relationship.Slug == "green-slime");
        Assert.Equal("/media/imported/bronzeSword.png", entry.Image);
    }

    [Fact]
    public void ParseItem_UsesFilenameFallbackForEmptyLegacyFile()
    {
        var path = Path.Combine(LegacyRoot, "items", "tools", "keys", "bronzeKey.html");

        var entry = parser.ParseItem(path, LegacyRoot);

        Assert.Equal("bronze-key", entry.Slug);
        Assert.Equal("Bronze Key", entry.Title);
        Assert.Contains("has not been authored yet", entry.Summary);
        Assert.Equal("/media/imported/bronzeKey.png", entry.Image);
    }
}
