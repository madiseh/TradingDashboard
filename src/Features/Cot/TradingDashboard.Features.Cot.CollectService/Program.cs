using Microsoft.AspNetCore.HttpOverrides;
using TradingDashboard.Features.Cot.CollectService.Abstractions;
using TradingDashboard.Features.Cot.CollectService.Endpoints;
using TradingDashboard.Features.Cot.CollectService.Options;
using TradingDashboard.Features.Cot.CollectService.Services;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ApplicationName = typeof(Program).Assembly.FullName,
    ContentRootPath = AppContext.BaseDirectory
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(options =>
{
    options.DocumentName = "v1";
    options.Title = "Trading Dashboard COT Collect Service";
    options.Version = "v1";
});
builder.Services.AddHealthChecks();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var connectionString = builder.Configuration.GetConnectionString("CotDatabase");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'CotDatabase' is required.");
}

builder.Services.Configure<CotCollectorOptions>(builder.Configuration.GetSection(CotCollectorOptions.SectionName));
builder.Services.AddDbContext<CotDbContext>(options =>
    options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

builder.Services.AddHttpClient(nameof(CotCollectionCoordinator), (sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<CotCollectorOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(options.RequestTimeoutSeconds);
});

builder.Services.AddSingleton<ICotCollectionCoordinator, CotCollectionCoordinator>();
builder.Services.AddHostedService<CotCollectionBackgroundService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CotDbContext>();
    dbContext.Database.Migrate();
    CotInstrumentSeedBootstrapper.SeedMissingInstruments(dbContext);
}

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.MapHealthChecks("/health");
app.MapCotCollectEndpoints();

app.Run();

public partial class Program;
