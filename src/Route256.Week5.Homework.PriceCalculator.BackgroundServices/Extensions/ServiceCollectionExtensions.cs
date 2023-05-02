using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Route256.Week5.Homework.PriceCalculator.BackgroundServices.Messages;
using Route256.Week5.Homework.PriceCalculator.BackgroundServices.Options;
using Route256.Week5.Homework.PriceCalculator.BackgroundServices.Serializers;
using Route256.Week5.Homework.PriceCalculator.BackgroundServices.Services;

namespace Route256.Week5.Homework.PriceCalculator.BackgroundServices.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackgroundServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .Configure<PriceCalculatorHostedServiceOptions>(
                configuration.GetSection("PriceCalculatorHostedServiceOptions"))
            .Configure<AnomalyFinderHostedServiceOptions>(
                configuration.GetSection("AnomalyFinderHostedServiceOptions"))
            .AddTransient(CreateCalculationRequestConsumer)
            .AddTransient(CreateCalculationResultConsumer)
            .AddTransient(CreateCalculationResultProducer)
            .AddTransient(CreateCalculationRequestProducer)
            .AddTransient(CreateUniversalProducer)
            .AddHostedService<PriceCalculatorHostedService>()
            .AddHostedService<AnomalyFinderHostedService>();
        return services;
    }

    private static IConsumer<long, CalculationRequestMessage> CreateCalculationRequestConsumer(
        IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<PriceCalculatorHostedServiceOptions>>();
        return new ConsumerBuilder<long, CalculationRequestMessage>(
                new ConsumerConfig
                {
                    BootstrapServers = options.Value.Broker,
                    GroupId = options.Value.GroupId,
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoCommit = true,
                    EnableAutoOffsetStore = false
                })
            .SetValueDeserializer(new JsonValueSerializer<CalculationRequestMessage>())
            .Build();
    }
    private static IConsumer<long, CalculationResultMessage> CreateCalculationResultConsumer(
        IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<AnomalyFinderHostedServiceOptions>>();
        return new ConsumerBuilder<long, CalculationResultMessage>(
                new ConsumerConfig
                {
                    BootstrapServers = options.Value.Broker,
                    GroupId = options.Value.GroupId,
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoCommit = true,
                    EnableAutoOffsetStore = false
                })
            .SetValueDeserializer(new JsonValueSerializer<CalculationResultMessage>())
            .Build();
    }

    private static IProducer<long, CalculationResultMessage> CreateCalculationResultProducer(
        IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<PriceCalculatorHostedServiceOptions>>();
        return new ProducerBuilder<long, CalculationResultMessage>(
                new ProducerConfig
                {
                    BootstrapServers = options.Value.Broker,
                    Acks = Acks.All
                })
            .SetValueSerializer(new JsonValueSerializer<CalculationResultMessage>())
            .Build();
    }

    private static IProducer<long, CalculationRequestMessage> CreateCalculationRequestProducer(
        IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<PriceCalculatorHostedServiceOptions>>();
        return new ProducerBuilder<long, CalculationRequestMessage>(
                new ProducerConfig
                {
                    BootstrapServers = options.Value.Broker,
                    Acks = Acks.All
                })
            .SetValueSerializer(new JsonValueSerializer<CalculationRequestMessage>())
            .Build();
    }

    private static IProducer<byte[], byte[]> CreateUniversalProducer(
        IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<PriceCalculatorHostedServiceOptions>>();
        return new ProducerBuilder<byte[], byte[]>(
                new ProducerConfig
                {
                    BootstrapServers = options.Value.Broker,
                    Acks = Acks.All,
                })
            .Build();
    }
}