using TradingDashboard.Infrastructure.Persistence.Implementation.Seed;
using TradingDashboard.Presentation.Api.Cot.DependencyInjection;
using TradingDashboard.Presentation.Api.Cot.Endpoints;

var builder = Program.CreateHostBuilder(args).Build();
var app = Program.ConfigureApp(builder);
app.Run();

public partial class Program
{
    public static WebApplicationBuilder CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = args,
            ApplicationName = typeof(Program).Assembly.FullName,
            ContentRootPath = AppContext.BaseDirectory
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new() { Title = "Trading Dashboard API - COT", Version = "v1" });
        });
        builder.Services.AddHealthChecks();

        builder.Services.AddIocRegistration(builder.Configuration);

        return builder;
    }

    public static WebApplication ConfigureApp(WebApplication app)
    {
        var skipDbInit = string.Equals(
            Environment.GetEnvironmentVariable("TD_SKIP_DB_INIT"),
            "true",
            StringComparison.OrdinalIgnoreCase);

        if (!skipDbInit)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CotDbContext>();
            dbContext.Database.Migrate();
            CotInstrumentSeedBootstrapper.SeedMissingInstruments(dbContext);
        }

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
        app.MapHealthChecks(RoutePaths.ApiCommon.Health);
        app.MapCotEndpoints();

        return app;
    }
}

