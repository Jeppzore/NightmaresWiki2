using Microsoft.AspNetCore.Mvc;
using NightmaresWiki.Api.Controllers;
using NightmaresWiki.Api.Dtos;
using NightmaresWiki.Api.Models;
using NightmaresWiki.Api.Repositories;

namespace NightmaresWiki.Api.Tests.Integration;

public sealed class ApiControllerTests
{
    private readonly IWikiEntryRepository repository;

    public ApiControllerTests()
    {
        repository = new FakeWikiEntryRepository([
            new WikiEntry
            {
                Type = "enemy",
                Slug = "green-slime",
                Title = "Green Slime",
                Summary = "A green slime.",
                Taxonomy = ["Enemies"],
                TaxonomyKeys = ["enemies"],
                Image = "/media/imported/greenslimegif.gif",
                IsDetailPage = true
            },
            new WikiEntry
            {
                Type = "item",
                Slug = "bronze-sword",
                Title = "Bronze Sword",
                Summary = "A good starter weapon.",
                Taxonomy = ["Items", "Weapons", "Melee"],
                TaxonomyKeys = ["items", "weapons", "melee"],
                Image = "/media/imported/bronzeSword.png",
                IsDetailPage = true
            },
            new WikiEntry
            {
                Type = "npc",
                Slug = "npcs",
                Title = "NPC",
                Summary = "Sparse NPC section metadata.",
                Taxonomy = ["NPC"],
                TaxonomyKeys = ["npc"],
                IsDetailPage = false
            }
        ]);
    }

    [Fact]
    public async Task HomeController_ReturnsSectionCounts()
    {
        var controller = new HomeController(repository);

        var result = await controller.Get(CancellationToken.None);
        var payload = Assert.IsType<OkObjectResult>(result.Result).Value as HomeResponseDto;

        Assert.NotNull(payload);
        Assert.Contains(payload.Sections, section => section.Type == "enemy" && section.Count == 1);
        Assert.Contains(payload.Sections, section => section.Type == "item" && section.Count == 1);
    }

    [Fact]
    public async Task EntriesController_FiltersByType()
    {
        var controller = new EntriesController(repository);

        var result = await controller.GetEntries("item", null, CancellationToken.None);
        var payload = Assert.IsType<OkObjectResult>(result.Result).Value as IReadOnlyList<WikiEntrySummaryDto>;

        Assert.NotNull(payload);
        Assert.Single(payload);
        Assert.Equal("bronze-sword", payload[0].Slug);
    }

    [Fact]
    public async Task EntriesController_ReturnsNotFoundForMissingSlug()
    {
        var controller = new EntriesController(repository);

        var result = await controller.GetEntry("missing-entry", CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task SearchController_ReturnsMatches()
    {
        var controller = new SearchController(repository);

        var result = await controller.Search("bronze", CancellationToken.None);
        var payload = Assert.IsType<OkObjectResult>(result.Result).Value as IReadOnlyList<WikiEntrySummaryDto>;

        Assert.NotNull(payload);
        Assert.Single(payload);
        Assert.Equal("Bronze Sword", payload[0].Title);
    }
}

internal sealed class FakeWikiEntryRepository(List<WikiEntry> entries) : IWikiEntryRepository
{
    public Task<long> CountAsync(string type, bool detailsOnly, CancellationToken cancellationToken)
    {
        var count = entries.Count(entry => entry.Type == type && (!detailsOnly || entry.IsDetailPage));
        return Task.FromResult((long)count);
    }

    public Task<WikiEntry?> GetBySlugAsync(string slug, CancellationToken cancellationToken) =>
        Task.FromResult(entries.FirstOrDefault(entry => entry.Slug == slug));

    public Task<List<WikiEntry>> GetEntriesAsync(string? type, string? category, bool includeMetadataOnly, CancellationToken cancellationToken)
    {
        var query = entries.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(type))
        {
            query = query.Where(entry => entry.Type == type);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(entry => entry.TaxonomyKeys.Contains(category));
        }

        if (!includeMetadataOnly)
        {
            query = query.Where(entry => entry.IsDetailPage);
        }

        return Task.FromResult(query.ToList());
    }

    public Task<List<WikiEntry>> GetHighlightsAsync(CancellationToken cancellationToken) =>
        Task.FromResult(entries.ToList());

    public Task<WikiEntry?> GetNpcSectionAsync(CancellationToken cancellationToken) =>
        Task.FromResult(entries.FirstOrDefault(entry => entry.Type == "npc"));

    public Task ReplaceAllAsync(IEnumerable<WikiEntry> entries, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task<List<WikiEntry>> SearchAsync(string query, CancellationToken cancellationToken)
    {
        var matches = entries
            .Where(entry => entry.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Task.FromResult(matches);
    }
}
