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

var skipDbInit = string.Equals(
    Environment.GetEnvironmentVariable("TD_SKIP_DB_INIT"),
    "true",
    StringComparison.OrdinalIgnoreCase);

if (!skipDbInit)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<CotDbContext>();
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
app.MapHealthChecks(RoutePaths.ApiCommon.Health);



app.Run();

