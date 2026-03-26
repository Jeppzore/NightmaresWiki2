using Microsoft.Extensions.FileProviders;
using MongoDB.Driver;
using NightmaresWiki.Api.Options;
using NightmaresWiki.Api.Repositories;
using NightmaresWiki.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoOptions>(builder.Configuration.GetSection(MongoOptions.SectionName));
builder.Services.Configure<ImportOptions>(builder.Configuration.GetSection(ImportOptions.SectionName));

builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var options = serviceProvider
        .GetRequiredService<IConfiguration>()
        .GetSection(MongoOptions.SectionName)
        .Get<MongoOptions>() ?? new MongoOptions();

    return new MongoClient(options.ConnectionString);
});

builder.Services.AddSingleton<IWikiEntryRepository, MongoWikiEntryRepository>();
builder.Services.AddSingleton<LegacyWikiParser>();
builder.Services.AddSingleton<IContentImportService, LegacyWikiImportService>();
builder.Services.AddHostedService<StartupImportHostedService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:4173", "http://localhost:5173", "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors();
var webRootPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot");
Directory.CreateDirectory(webRootPath);
app.Environment.WebRootPath = webRootPath;
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(webRootPath)
});
app.MapControllers();

app.MapGet("/", () => Results.Ok(new
{
    name = "NightmaresWiki2 API",
    endpoints = new[]
    {
        "/api/home",
        "/api/search?q=",
        "/api/entries?type=&category=",
        "/api/entries/{slug}"
    }
}));

app.Run();

public partial class Program;
