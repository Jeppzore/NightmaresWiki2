using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using NightmaresWiki.Api.Models;
using NightmaresWiki.Api.Options;

namespace NightmaresWiki.Api.Repositories;

public sealed class MongoWikiEntryRepository : IWikiEntryRepository
{
    private readonly IMongoCollection<WikiEntry> collection;

    public MongoWikiEntryRepository(IMongoClient mongoClient, IOptions<MongoOptions> options)
    {
        var database = mongoClient.GetDatabase(options.Value.DatabaseName);
        collection = database.GetCollection<WikiEntry>("entries");

        var slugIndex = Builders<WikiEntry>.IndexKeys.Ascending(entry => entry.Slug);
        collection.Indexes.CreateOne(new CreateIndexModel<WikiEntry>(slugIndex, new CreateIndexOptions { Unique = true }));
    }

    public async Task ReplaceAllAsync(IEnumerable<WikiEntry> entries, CancellationToken cancellationToken)
    {
        await collection.DeleteManyAsync(FilterDefinition<WikiEntry>.Empty, cancellationToken);

        var entryList = entries.ToList();
        if (entryList.Count > 0)
        {
            await collection.InsertManyAsync(entryList, cancellationToken: cancellationToken);
        }
    }

    public async Task<List<WikiEntry>> GetEntriesAsync(string? type, string? category, bool includeMetadataOnly, CancellationToken cancellationToken)
    {
        var filters = new List<FilterDefinition<WikiEntry>>();

        if (!string.IsNullOrWhiteSpace(type))
        {
            filters.Add(Builders<WikiEntry>.Filter.Eq(entry => entry.Type, type));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            filters.Add(Builders<WikiEntry>.Filter.AnyEq(entry => entry.TaxonomyKeys, category));
        }

        if (!includeMetadataOnly)
        {
            filters.Add(Builders<WikiEntry>.Filter.Eq(entry => entry.IsDetailPage, true));
        }

        var filter = filters.Count == 0
            ? FilterDefinition<WikiEntry>.Empty
            : Builders<WikiEntry>.Filter.And(filters);

        return await collection.Find(filter)
            .SortBy(entry => entry.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<WikiEntry?> GetBySlugAsync(string slug, CancellationToken cancellationToken) =>
        await collection.Find(entry => entry.Slug == slug).FirstOrDefaultAsync(cancellationToken);

    public async Task<List<WikiEntry>> SearchAsync(string query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return [];
        }

        var regex = new BsonRegularExpression(query.Trim(), "i");
        var filter = Builders<WikiEntry>.Filter.Or(
            Builders<WikiEntry>.Filter.Regex(entry => entry.Title, regex),
            Builders<WikiEntry>.Filter.Regex(entry => entry.Summary, regex));

        return await collection.Find(filter)
            .SortBy(entry => entry.Title)
            .Limit(12)
            .ToListAsync(cancellationToken);
    }

    public async Task<long> CountAsync(string type, bool detailsOnly, CancellationToken cancellationToken)
    {
        var filter = Builders<WikiEntry>.Filter.Eq(entry => entry.Type, type);
        if (detailsOnly)
        {
            filter &= Builders<WikiEntry>.Filter.Eq(entry => entry.IsDetailPage, true);
        }

        return await collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
    }

    public async Task<List<WikiEntry>> GetHighlightsAsync(CancellationToken cancellationToken)
    {
        var filter = Builders<WikiEntry>.Filter.Or(
            Builders<WikiEntry>.Filter.And(
                Builders<WikiEntry>.Filter.Eq(entry => entry.Type, "enemy"),
                Builders<WikiEntry>.Filter.Eq(entry => entry.IsDetailPage, true)),
            Builders<WikiEntry>.Filter.And(
                Builders<WikiEntry>.Filter.Eq(entry => entry.Type, "item"),
                Builders<WikiEntry>.Filter.Eq(entry => entry.IsDetailPage, true)),
            Builders<WikiEntry>.Filter.Eq(entry => entry.Slug, "npcs"));

        return await collection.Find(filter)
            .SortBy(entry => entry.Type)
            .Limit(6)
            .ToListAsync(cancellationToken);
    }

    public async Task<WikiEntry?> GetNpcSectionAsync(CancellationToken cancellationToken) =>
        await collection.Find(entry => entry.Slug == "npcs").FirstOrDefaultAsync(cancellationToken);
}
