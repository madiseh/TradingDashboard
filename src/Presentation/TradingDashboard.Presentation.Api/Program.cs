using TradingDashboard.Infrastructure.Persistence.Implementation.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

var app = builder.Build();

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

app.Run();
