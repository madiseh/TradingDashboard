using Microsoft.AspNetCore.HttpOverrides;
using TradingDashboard.Features.CurrencyStrength.CollectService.Abstractions;
using TradingDashboard.Features.CurrencyStrength.CollectService.Endpoints;
using TradingDashboard.Features.CurrencyStrength.CollectService.Options;
using TradingDashboard.Features.CurrencyStrength.CollectService.Services;

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
    options.Title = "Trading Dashboard Currency Strength Collect Service";
    options.Version = "v1";
});
builder.Services.AddHealthChecks();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.Configure<CurrencyStrengthCollectorOptions>(
    builder.Configuration.GetSection(CurrencyStrengthCollectorOptions.SectionName));
builder.Services.AddHttpClient(nameof(CurrencyStrengthCollectionCoordinator), (sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<CurrencyStrengthCollectorOptions>>().Value;
    client.Timeout = TimeSpan.FromSeconds(options.RequestTimeoutSeconds);
});

builder.Services.AddSingleton<ICurrencyStrengthCollectionCoordinator, CurrencyStrengthCollectionCoordinator>();
builder.Services.AddHostedService<CurrencyStrengthCollectionBackgroundService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.MapHealthChecks("/health");
app.MapCurrencyStrengthCollectEndpoints();

app.Run();

public partial class Program;
