using NightmaresWiki.Api.Models;

namespace NightmaresWiki.Api.Repositories;

public interface IWikiEntryRepository
{
    Task ReplaceAllAsync(IEnumerable<WikiEntry> entries, CancellationToken cancellationToken);

    Task<List<WikiEntry>> GetEntriesAsync(string? type, string? category, bool includeMetadataOnly, CancellationToken cancellationToken);

    Task<WikiEntry?> GetBySlugAsync(string slug, CancellationToken cancellationToken);

    Task<List<WikiEntry>> SearchAsync(string query, CancellationToken cancellationToken);

    Task<long> CountAsync(string type, bool detailsOnly, CancellationToken cancellationToken);

    Task<List<WikiEntry>> GetHighlightsAsync(CancellationToken cancellationToken);

    Task<WikiEntry?> GetNpcSectionAsync(CancellationToken cancellationToken);
}
