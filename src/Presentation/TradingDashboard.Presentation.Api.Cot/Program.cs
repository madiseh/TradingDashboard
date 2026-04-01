using TradingDashboard.Infrastructure.Persistence.Implementation.Seed;
using TradingDashboard.Presentation.Api.Cot.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddOpenApiDocument(options =>
{
    options.DocumentName = "v1";
    options.Title = "Trading Dashboard API - COT";
    options.Version = "v1";
});
builder.Services.AddHealthChecks();
Directory.CreateDirectory(Path.Combine(builder.Environment.ContentRootPath, "db"));

builder.Services.AddIocRegistration(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CotDbContext>();
    dbContext.Database.EnsureCreated();
    CotInstrumentSeedBootstrapper.SeedMissingInstruments(dbContext);
}

// Configure Swagger
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapHealthChecks(RoutePaths.ApiCommon.Health);



app.Run();

