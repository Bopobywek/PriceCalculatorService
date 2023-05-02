using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Route256.Week5.Homework.PriceCalculator.ProducerApp.Interfaces;
using Route256.Week5.Homework.PriceCalculator.ProducerApp.Options;

namespace Route256.Week5.Homework.PriceCalculator.ProducerApp;

internal static class ApplicationStartup
{
    public static async Task Main()
    {
        IContext context = new Context();
        var cancellationTokenSource = new CancellationTokenSource();
        
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(context.GetProjectDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var serviceCollection = new ServiceCollection();
        serviceCollection
            .AddTransient<ProducerApp>()
            .Configure<ProducerAppOptions>(configuration.GetSection("ProducerAppOptions"))
            .Configure<RandomOptions>(configuration.GetSection("RandomOptions"))
            .AddLogging(builder => builder.AddConfiguration(configuration.GetSection("Logging")).AddConsole());
        
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var app = serviceProvider.GetRequiredService<ProducerApp>();
        
        await app.Run(cancellationTokenSource.Token);
    }
}