using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WindowDisplayService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "ZackDisplayService2";
});
builder.Services.AddHostedService<WindowPlacementWorker>();

await builder.Build().RunAsync();
