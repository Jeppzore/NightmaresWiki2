using Microsoft.AspNetCore.Mvc;
using NightmaresWiki.Api.Dtos;
using NightmaresWiki.Api.Repositories;

namespace NightmaresWiki.Api.Controllers;

[ApiController]
[Route("api/entries")]
public sealed class EntriesController(IWikiEntryRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<WikiEntrySummaryDto>>> GetEntries(
        [FromQuery] string? type,
        [FromQuery] string? category,
        CancellationToken cancellationToken)
    {
        var includeMetadataOnly = string.Equals(type, "npc", StringComparison.OrdinalIgnoreCase);
        var entries = await repository.GetEntriesAsync(type, category, includeMetadataOnly, cancellationToken);
        return Ok(entries.Select(entry => entry.ToSummaryDto()).ToList());
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<WikiEntryDetailDto>> GetEntry(string slug, CancellationToken cancellationToken)
    {
        var entry = await repository.GetBySlugAsync(slug, cancellationToken);
        if (entry is null)
        {
            return NotFound();
        }

        return Ok(entry.ToDetailDto());
    }
}
