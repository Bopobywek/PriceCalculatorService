using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Route256.Week5.Homework.PriceCalculator.BackgroundServices.Messages;
using Route256.Week5.Homework.PriceCalculator.BackgroundServices.Options;
using Route256.Week5.Homework.PriceCalculator.Bll.Commands;

namespace Route256.Week5.Homework.PriceCalculator.BackgroundServices.Services;

public class AnomalyFinderHostedService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly IOptionsMonitor<AnomalyFinderHostedServiceOptions> _optionsMonitor;
    private readonly IConsumer<long, CalculationResultMessage> _consumer;

    public AnomalyFinderHostedService(IServiceProvider services,
        IOptionsMonitor<AnomalyFinderHostedServiceOptions> optionsMonitor,
        IConsumer<long, CalculationResultMessage> consumer)
    {
        _services = services;
        _optionsMonitor = optionsMonitor;
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await BackgroundProcessing(stoppingToken);
    }

    private async Task BackgroundProcessing(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_optionsMonitor.CurrentValue.CalculationResultTopic);

        while (!stoppingToken.IsCancellationRequested)
        {
            var result = await Task.Run(() => _consumer.Consume(stoppingToken), stoppingToken);

            if (result.Message.Value.Price > _optionsMonitor.CurrentValue.NormalPriceThreshold)
            {
                var command = new SaveAnomalyPriceCommand(
                    result.Message.Value.GoodId,
                    result.Message.Value.Price);
                
                await using (var scope = _services.CreateAsyncScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await mediator.Send(command, stoppingToken);
                }
            }

            _consumer.StoreOffset(result);
        }
    }
}