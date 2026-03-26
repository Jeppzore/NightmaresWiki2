namespace NightmaresWiki.Api.Services;

public interface IContentImportService
{
    Task<ImportSummary> ImportAsync(CancellationToken cancellationToken);
}
