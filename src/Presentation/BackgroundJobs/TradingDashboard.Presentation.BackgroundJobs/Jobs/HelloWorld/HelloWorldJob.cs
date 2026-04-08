using Quartz;

namespace TradingDashboard.Presentation.BackgroundJobs.Jobs.HelloWorld;

public sealed class HelloWorldJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine("HELLO WORLD");
        return Task.CompletedTask;
    }
}
