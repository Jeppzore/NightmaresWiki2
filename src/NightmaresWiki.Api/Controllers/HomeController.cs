using Microsoft.AspNetCore.Mvc;
using NightmaresWiki.Api.Dtos;
using NightmaresWiki.Api.Repositories;

namespace NightmaresWiki.Api.Controllers;

[ApiController]
[Route("api/home")]
public sealed class HomeController(IWikiEntryRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<HomeResponseDto>> Get(CancellationToken cancellationToken)
    {
        var enemyCount = await repository.CountAsync("enemy", true, cancellationToken);
        var itemCount = await repository.CountAsync("item", true, cancellationToken);
        var npcCount = await repository.CountAsync("npc", true, cancellationToken);
        var npcSection = await repository.GetNpcSectionAsync(cancellationToken);
        var highlights = await repository.GetHighlightsAsync(cancellationToken);

        var response = new HomeResponseDto
        {
            Title = "Nightmares Wiki",
            Intro = "A Nightmares-first field guide inspired by classic fantasy wiki layouts. Explore enemies, equipment, and the still-growing cast of NPCs imported from the legacy Nightmares wiki.",
            Sections = new[]
            {
                new HomeSectionDto
                {
                    Type = "enemy",
                    Title = "Enemies",
                    Route = "/enemies",
                    Description = "Combat-focused creature pages with stats, lore, and cross-linked drops.",
                    Count = (int)enemyCount,
                    Image = highlights.FirstOrDefault(entry => entry.Type == "enemy")?.Image
                },
                new HomeSectionDto
                {
                    Type = "item",
                    Title = "Items",
                    Route = "/items",
                    Description = "Weapons, armor, keys, and valuables grouped by Nightmares categories from the legacy repository.",
                    Count = (int)itemCount,
                    Image = highlights.FirstOrDefault(entry => entry.Type == "item")?.Image
                },
                new HomeSectionDto
                {
                    Type = "npc",
                    Title = "NPC",
                    Route = "/npcs",
                    Description = "A placeholder section that keeps NPCs visible in the IA while the source repo catches up with dedicated articles.",
                    Count = (int)npcCount,
                    Image = npcSection?.Image,
                    StatusNote = "No NPC detail pages exist in the legacy source yet."
                }
            },
            Highlights = highlights.Select(entry => entry.ToSummaryDto()).ToList()
        };

        return Ok(response);
    }
}
