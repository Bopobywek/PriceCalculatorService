using System.IO;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Route256.Week5.Homework.PriceCalculator.Dal.Extensions;
using Route256.Week5.Homework.PriceCalculator.Dal.Repositories.Interfaces;
using Route256.Week5.Homework.PriceCalculator.Dal.Settings;

namespace Route256.Week5.Homework.PriceCalculator.IntegrationTests.Fixtures
{
    public class TestFixture
    {
        public ICalculationRepository CalculationRepository { get; }

        public IGoodsRepository GoodsRepository { get; }
        public IAnomaliesRepository AnomaliesRepository { get; }
        public DalOptions DalSettings { get; }

        public TestFixture()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddDalInfrastructure(config)
                        .AddDalRepositories();
                })
                .Build();
            
            ClearDatabase(host);
            host.MigrateUp();

            var serviceProvider = host.Services;
            CalculationRepository = serviceProvider.GetRequiredService<ICalculationRepository>();
            GoodsRepository = serviceProvider.GetRequiredService<IGoodsRepository>();
            AnomaliesRepository = serviceProvider.GetRequiredService<IAnomaliesRepository>();
            DalSettings = serviceProvider.GetRequiredService<IOptions<DalOptions>>().Value;
        }

        private static void ClearDatabase(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateDown(20230301);
        }
    }
}