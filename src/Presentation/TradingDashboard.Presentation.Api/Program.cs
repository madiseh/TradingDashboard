using TradingDashboard.Application.Features.Auth.Commands.SignIn;
using TradingDashboard.Application.Common.Security;
using TradingDashboard.Infrastructure.Persistence.Implementation.Context;
using TradingDashboard.Infrastructure.Persistence.Implementation.Seed;
using TradingDashboard.Presentation.Api.Cot.Endpoints;
using TradingDashboard.Presentation.Api.CurrencyStrength.Endpoints;
using TradingDashboard.Presentation.Api.UserManagement.Endpoints;
using TradingDashboard.Presentation.Api.UserManagement.Options;
using TradingDashboard.Presentation.Api.DependencyInjection;

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
    options.Title = "Trading Dashboard API";
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

var jwtOptions = JwtOptions.ReadAndValidate(builder.Configuration);
var keyBytes = Encoding.UTF8.GetBytes(jwtOptions.SigningKey);
var authenticationBuilder = builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ClockSkew = TimeSpan.Zero
        };
    })
    .AddCookie("External", options =>
    {
        options.Cookie.Name = "__td_login_external";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        options.SlidingExpiration = false;
    });

var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
if (!string.IsNullOrWhiteSpace(googleClientId)
    && !string.IsNullOrWhiteSpace(googleClientSecret))
{
    authenticationBuilder.AddGoogle("Google", options =>
    {
        options.SignInScheme = "External";
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
    });
}

builder.Services.AddAuthorization();

var app = builder.Build();
var skipDbInit = string.Equals(
    Environment.GetEnvironmentVariable("TD_SKIP_DB_INIT"),
    "true",
    StringComparison.OrdinalIgnoreCase);

if (!skipDbInit)
{
    using var scope = app.Services.CreateScope();
    var cotDbContext = scope.ServiceProvider.GetRequiredService<CotDbContext>();
    var userManagementDbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();
    var passwordHashingService = scope.ServiceProvider.GetRequiredService<IPasswordHashingService>();

    cotDbContext.Database.Migrate();
    CotInstrumentSeedBootstrapper.SeedMissingInstruments(cotDbContext);
    userManagementDbContext.Database.Migrate();
    AdminUserSeedBootstrapper.SeedMissingAdmin(userManagementDbContext, passwordHashingService);
}

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks(RoutePaths.ApiCommon.Health);
app.MapCotEndpoints();
app.MapCurrencyStrengthEndpoints();
app.MapAuthEndpoints();
app.MapAdminUserEndpoints();
app.MapSubscriptionEndpoints();

await app.RunAsync();

public partial class Program;
