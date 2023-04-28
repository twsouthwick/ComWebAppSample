using System.Runtime.InteropServices;
using Windows.Win32;

namespace InProcessComWebApp;

public static class ComExampleExtensions
{
    public static void AddComExample(this IServiceCollection services)
    {
        services.AddHostedService<ComBackgroundService>();
    }

    public static IEndpointConventionBuilder MapComExample(this IEndpointRouteBuilder endpoint)
        => endpoint.MapGet("/", () =>
        {
            return ComFactory<ComObject>.Retrieve();
        });

    private sealed class ComBackgroundService : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
            => Task.Run(() =>
            {
                ComFactory<ComObject>.Register(stoppingToken);
            }, stoppingToken);
    }

    [Guid("F35856B9-ED77-43DF-BDD1-1C54CF60E27C")]
    [ComVisible(true)]
    public class ComObject
    {
        private static int _counter;

        public int Counter { get; } = Interlocked.Increment(ref _counter);
    }
}
