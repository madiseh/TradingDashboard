using Microsoft.Extensions.Hosting;

namespace TradingDashboard.Presentation.Api.Login;

public static class SwaggerHostFactory
{
    public static IHost CreateHost()
    {
        var builder = Program.CreateHostBuilder(Array.Empty<string>());
        var app = Program.ConfigureApp(builder.Build());
        return app;
    }
}
