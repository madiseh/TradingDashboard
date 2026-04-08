using Microsoft.AspNetCore.HttpOverrides;
using TradingDashboard.Presentation.Api.CurrencyStrength.DependencyInjection;
using TradingDashboard.Presentation.Api.CurrencyStrength.Endpoints;

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
    options.Title = "Trading Dashboard API - Currency Strength";
    options.Version = "v1";
});
builder.Services.AddHealthChecks();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddIocRegistration(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.MapHealthChecks(RoutePaths.ApiCommon.Health);
app.MapCurrencyStrengthEndpoints();

app.Run();

public partial class Program;
