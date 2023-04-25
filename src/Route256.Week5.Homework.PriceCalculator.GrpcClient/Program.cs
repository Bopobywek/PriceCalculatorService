using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Route256.Week5.Homework.PriceCalculator.GrpcClient.Interfaces;
using Route256.Week5.Homework.PriceCalculator.GrpcClient.Options;

namespace Route256.Week5.Homework.PriceCalculator.GrpcClient;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        IContext context = new Context();
        var cancellationTokenSource = new CancellationTokenSource();
        
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(context.GetProjectDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var serviceCollection = new ServiceCollection();
        serviceCollection
            .AddTransient<ClientApp>()
            .AddTransient<IContext, Context>()
            .Configure<ClientOptions>(configuration.GetSection("ClientOptions"));
        
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var app = serviceProvider.GetRequiredService<ClientApp>();
        
        await app.Run(cancellationTokenSource.Token);
    }
}