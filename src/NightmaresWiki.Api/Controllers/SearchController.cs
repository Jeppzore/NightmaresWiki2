using Microsoft.AspNetCore.Mvc;
using NightmaresWiki.Api.Dtos;
using NightmaresWiki.Api.Repositories;

namespace NightmaresWiki.Api.Controllers;

[ApiController]
[Route("api/search")]
public sealed class SearchController(IWikiEntryRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<WikiEntrySummaryDto>>> Search([FromQuery] string? q, CancellationToken cancellationToken)
    {
        var entries = await repository.SearchAsync(q ?? string.Empty, cancellationToken);
        return Ok(entries.Select(entry => entry.ToSummaryDto()).ToList());
    }
}
