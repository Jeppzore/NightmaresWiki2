using NightmaresWiki.Api.Models;

namespace NightmaresWiki.Api.Dtos;

public static class DtoMappings
{
    public static WikiEntrySummaryDto ToSummaryDto(this WikiEntry entry) =>
        new()
        {
            Type = entry.Type,
            Slug = entry.Slug,
            Title = entry.Title,
            Summary = entry.Summary,
            Taxonomy = entry.Taxonomy,
            Image = entry.Image,
            IsDetailPage = entry.IsDetailPage
        };

    public static WikiEntryDetailDto ToDetailDto(this WikiEntry entry) =>
        new()
        {
            Type = entry.Type,
            Slug = entry.Slug,
            Title = entry.Title,
            Summary = entry.Summary,
            Taxonomy = entry.Taxonomy,
            Image = entry.Image,
            IsDetailPage = entry.IsDetailPage,
            SourcePath = entry.SourcePath,
            BodySections = entry.BodySections.Select(section => new BodySectionDto
            {
                Heading = section.Heading,
                Content = section.Content,
                Items = section.Items
            }).ToList(),
            Stats = entry.Stats.Select(stat => new WikiStatDto
            {
                Label = stat.Label,
                Value = stat.Value
            }).ToList(),
            Relationships = entry.Relationships.Select(relationship => new EntryRelationshipDto
            {
                Type = relationship.Type,
                Title = relationship.Title,
                Slug = relationship.Slug,
                Image = relationship.Image
            }).ToList()
        };
}
