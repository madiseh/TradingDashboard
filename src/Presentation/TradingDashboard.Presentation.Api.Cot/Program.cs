using TradingDashboard.Infrastructure.Persistence.Implementation.Seed;
using TradingDashboard.Presentation.Api.Cot.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Trading Dashboard API - COT", Version = "v1" });
});
builder.Services.AddHealthChecks();
Directory.CreateDirectory(Path.Combine(builder.Environment.ContentRootPath, "db"));

builder.Services.AddIocRegistration(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TradingDashboardDbContext>();
    dbContext.Database.EnsureCreated();
    CotInstrumentSeedBootstrapper.SeedMissingInstruments(dbContext);
}

// Configure Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Trading Dashboard API v1");
        options.RoutePrefix = string.Empty; 
    });
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapHealthChecks(RoutePaths.Api.Health);

// Market Endpoints
var markets = CommonMarketSeedData.Markets;
var marketTypes = markets
    .Select(x => x.Type)
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .OrderBy(x => x)
    .ToArray();

app.MapGet(RoutePaths.Api.MarketTypes, () =>
{
    return Results.Ok(new { items = marketTypes });
})
.WithName("GetMarketTypes")
.WithTags("Common");

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
})
.WithName("GetMarkets")
.WithTags("Common");

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
})
.WithName("GetMarketsDirectory")
.WithTags("Common");

app.Run();

