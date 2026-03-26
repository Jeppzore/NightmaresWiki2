using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NightmaresWiki.Api.Models;

public sealed class WikiEntry
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public List<BodySection> BodySections { get; set; } = [];

    public List<WikiStat> Stats { get; set; } = [];

    public List<string> Taxonomy { get; set; } = [];

    public List<string> TaxonomyKeys { get; set; } = [];

    public List<EntryRelationship> Relationships { get; set; } = [];

    public string? Image { get; set; }

    public string SourcePath { get; set; } = string.Empty;

    public bool IsDetailPage { get; set; }

    public DateTime ImportedAtUtc { get; set; }
}
