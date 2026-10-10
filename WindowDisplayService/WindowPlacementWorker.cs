using Microsoft.Extensions.Hosting;
using WindowPlacementLib;

namespace WindowDisplayService;

internal sealed class WindowPlacementWorker : BackgroundService
{
    private DisplayWatcher? displayWatcher;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        displayWatcher = new DisplayWatcher();

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
    }

    public override void Dispose()
    {
        displayWatcher?.Dispose();
        base.Dispose();
    }
}
