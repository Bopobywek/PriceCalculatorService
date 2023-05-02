using System.Text;
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Route256.Week5.Homework.PriceCalculator.BackgroundServices.Messages;
using Route256.Week5.Homework.PriceCalculator.BackgroundServices.Options;
using Route256.Week5.Homework.PriceCalculator.BackgroundServices.Validators;
using Route256.Week5.Homework.PriceCalculator.Bll.Commands;
using Route256.Week5.Homework.PriceCalculator.Bll.Models;

namespace Route256.Week5.Homework.PriceCalculator.BackgroundServices.Services;

public class PriceCalculatorHostedService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly IOptionsMonitor<PriceCalculatorHostedServiceOptions> _optionsMonitor;
    private readonly IConsumer<long, CalculationRequestMessage> _consumer;
    private readonly IProducer<long, CalculationResultMessage> _resultProducer;

    public PriceCalculatorHostedService(IServiceProvider services,
        IOptionsMonitor<PriceCalculatorHostedServiceOptions> optionsMonitor,
        IConsumer<long, CalculationRequestMessage> consumer,
        IProducer<long, CalculationResultMessage> resultProducer)
    {
        _services = services;
        _optionsMonitor = optionsMonitor;
        _consumer = consumer;
        _resultProducer = resultProducer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await BackgroundProcessing(stoppingToken);
    }

    private async Task<CalculateGoodDeliveryPriceResult> CalculatePrice(
        CalculationRequestMessage calculationRequestMessage,
        CancellationToken stoppingToken)
    {
        var command = new CalculateGoodDeliveryPriceCommand(new GoodModel(
            calculationRequestMessage.Height,
            calculationRequestMessage.Length,
            calculationRequestMessage.Width,
            calculationRequestMessage.Weight));

        CalculateGoodDeliveryPriceResult calculateDeliveryPriceResult;

        using (var scope = _services.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            calculateDeliveryPriceResult = await mediator.Send(command, stoppingToken);
        }

        return calculateDeliveryPriceResult;
    }

    private async Task SendMessageToDeadLetterQueue<TKey, TValue>(
        ConsumeResult<TKey, TValue> result, string reason, CancellationToken stoppingToken)
    {
        var message = new Message<TKey, TValue>
        {
            Headers = new Headers()
            {
                {"ErrorReason", Encoding.Default.GetBytes(reason)}
            },
            Key = result.Message.Key,
            Value = result.Message.Value
        };

        var errorProducer = _services.GetRequiredService<IProducer<TKey, TValue>>();

        await errorProducer.ProduceAsync(
            _optionsMonitor.CurrentValue.DeadLetterQueueTopic,
            message,
            stoppingToken
        );
    }

    private async Task BackgroundProcessing(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_optionsMonitor.CurrentValue.CalculationRequestTopic);

        while (!stoppingToken.IsCancellationRequested)
        {
            ConsumeResult<long, CalculationRequestMessage> result;
            try
            {
                result = await Task.Run(() => _consumer.Consume(stoppingToken), stoppingToken);
            }
            catch (ConsumeException consumeException)
            {
                await SendMessageToDeadLetterQueue(consumeException.ConsumerRecord, consumeException.Error.Reason,
                    stoppingToken);

                var topicPartitionOffset = new TopicPartitionOffset(
                    consumeException.ConsumerRecord.TopicPartition,
                    consumeException.ConsumerRecord.Offset + 1,
                    consumeException.ConsumerRecord.LeaderEpoch);
                _consumer.StoreOffset(topicPartitionOffset);
                continue;
            }

            var validator = new CalculationRequestEventValidator();
            var validationResult = await validator.ValidateAsync(result.Message.Value, stoppingToken);

            if (!validationResult.IsValid)
            {
                await SendMessageToDeadLetterQueue(result, validationResult.ToString(),
                    stoppingToken);

                _consumer.StoreOffset(result);
                continue;
            }

            var calculationEventRequest = result.Message.Value;

            var calculateDeliveryPriceResult = await CalculatePrice(calculationEventRequest, stoppingToken);

            var resultMessage =
                new CalculationResultMessage(calculationEventRequest.GoodId, calculateDeliveryPriceResult.Price);

            await _resultProducer.ProduceAsync(
                _optionsMonitor.CurrentValue.CalculationResultTopic,
                new Message<long, CalculationResultMessage>
                {
                    Key = resultMessage.GoodId,
                    Value = resultMessage
                },
                stoppingToken);

            _consumer.StoreOffset(result);
        }

        _consumer.Close();
    }
}