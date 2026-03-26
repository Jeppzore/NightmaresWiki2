using Microsoft.Extensions.Options;
using NightmaresWiki.Api.Options;

namespace NightmaresWiki.Api.Services;

public sealed class StartupImportHostedService(
    IContentImportService contentImportService,
    IOptions<ImportOptions> importOptions,
    ILogger<StartupImportHostedService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!importOptions.Value.ImportOnStartup)
        {
            logger.LogInformation("NightmaresWiki import on startup is disabled.");
            return;
        }

        try
        {
            var summary = await contentImportService.ImportAsync(cancellationToken);
            logger.LogInformation(
                "Imported legacy Nightmares wiki content. Enemies: {Enemies}, Items: {Items}, NPC metadata records: {NpcRecords}",
                summary.Enemies,
                summary.Items,
                summary.NpcMetadataEntries);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "NightmaresWiki import failed during application startup.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
