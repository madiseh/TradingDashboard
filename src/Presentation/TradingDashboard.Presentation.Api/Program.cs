using TradingDashboard.Infrastructure.Persistence.Implementation.Seed;
using Microsoft.EntityFrameworkCore;
using TradingDashboard.Application.Repository.Cot;
using TradingDashboard.Infrastructure.Persistence.Implementation.Repositories;
using TradingDashboard.Infrastructure.Persistence.SimulatedData.Context;
using TradingDashboard.Infrastructure.Persistence.SimulatedData.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

builder.Services.AddDbContext<SimulatedDataDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("SimulatedData")
        ?? "Data Source=tradingdashboard.db";
    options.UseSqlite(connectionString);
});
builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<SimulatedDataDbContext>());

var useFakeRepository = builder.Configuration.GetValue<bool>("Persistence:UseFakeRepository", true);
if (useFakeRepository)
{
    builder.Services.AddSingleton<ICotInstrumentRepository, CotInstrumentFakeRepository>();
}
else
{
    builder.Services.AddScoped<ICotInstrumentRepository, CotInstrumentRepository>();
}

var app = builder.Build();

if (!useFakeRepository)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<SimulatedDataDbContext>();
    dbContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapHealthChecks(RoutePaths.Api.Health);

var markets = CommonMarketSeedData.Markets;
var marketTypes = markets
    .Select(x => x.Type)
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .OrderBy(x => x)
    .ToArray();

app.MapGet(RoutePaths.Api.MarketTypes, () =>
{
    return Results.Ok(new { items = marketTypes });
});

app.MapGet(RoutePaths.Api.Markets, (string type, string? q) =>
{
    if (string.IsNullOrWhiteSpace(type) || !marketTypes.Contains(type, StringComparer.OrdinalIgnoreCase))
    {
        return Results.BadRequest(new
        {
            error = new
            {
                code = "INVALID_TYPE",
                message = $"Unsupported market type '{type}'."
            }
        });
    }

    var items = markets
        .Where(x => string.Equals(x.Type, type, StringComparison.OrdinalIgnoreCase))
        .Where(x =>
            string.IsNullOrWhiteSpace(q) ||
            x.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
            x.Slug.Contains(q, StringComparison.OrdinalIgnoreCase))
        .Select(x => new
        {
            x.InstrumentId,
            x.Slug,
            x.Name,
            availableModels = x.AvailableModels
        })
        .ToArray();

    return Results.Ok(new
    {
        type,
        items
    });
});

app.MapGet(RoutePaths.Api.MarketsDirectory, () =>
{
    var groups = marketTypes
        .Select(type => new
        {
            type,
            items = markets
                .Where(x => string.Equals(x.Type, type, StringComparison.OrdinalIgnoreCase))
                .Select(x => new
                {
                    x.InstrumentId,
                    x.Slug,
                    x.Name,
                    availableModels = x.AvailableModels
                })
                .ToArray()
        })
        .ToArray();

    return Results.Ok(new { groups });
});

app.MapGet(RoutePaths.Api.CotInstrumentBySlug, (string instrumentSlug, ICotInstrumentRepository repository) =>
{
    var instrument = repository.GetBySlug(instrumentSlug);
    if (instrument is null)
    {
        return Results.NotFound(new
        {
            error = new
            {
                code = "INSTRUMENT_NOT_FOUND",
                message = $"Instrument '{instrumentSlug}' was not found."
            }
        });
    }

    return Results.Ok(new
    {
        id = instrument.Id,
        instrumentId = instrument.InstrumentId,
        instrumentSlug = instrument.InstrumentSlug,
        instrumentName = instrument.InstrumentName
    });
});

app.Run();

