// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using POI.ThirdParty.ScoreSaber.LiveFeed.Client;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(builder => builder.AddEnvironmentVariables())
    .ConfigureServices(services =>
    {
        services.AddHostedService<ScoreSaberWebSocketWorker>();
    })
    .Build();

await host.RunAsync();
