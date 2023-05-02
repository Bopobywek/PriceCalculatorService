using AutoBogus;
using Bogus;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Route256.Week5.Homework.PriceCalculator.ProducerApp.Interfaces;
using Route256.Week5.Homework.PriceCalculator.ProducerApp.Messages;
using Route256.Week5.Homework.PriceCalculator.ProducerApp.Options;

namespace Route256.Week5.Homework.PriceCalculator.ProducerApp;

public class ProducerApp : IProducerApp
{
    private readonly IOptionsSnapshot<ProducerAppOptions> _optionsSnapshot;
    private readonly IProducer<long, CalculationRequestMessage> _producer;

    private readonly Faker<CalculationRequestMessage> _faker;
    private readonly object _lock = new();

    public ProducerApp(IOptionsSnapshot<ProducerAppOptions> optionsSnapshot,
        IOptionsSnapshot<RandomOptions> randomOptions)
    {
        _optionsSnapshot = optionsSnapshot;
        _producer = new ProducerBuilder<long, CalculationRequestMessage>(
                new ProducerConfig
                {
                    BootstrapServers = optionsSnapshot.Value.Broker,
                    Acks = Acks.All
                })
            .SetValueSerializer(new JsonValueSerializer<CalculationRequestMessage>())
            .Build();

        _faker = new AutoFaker<CalculationRequestMessage>()
            .RuleFor(x => x.GoodId,
                f => f.Random.Long(0L))
            .RuleFor(x => x.Width,
                f => f.Random.Double(1, randomOptions.Value.MaxWidth))
            .RuleFor(x => x.Height,
                f => f.Random.Double(1, randomOptions.Value.MaxHeight))
            .RuleFor(x => x.Length,
                f => f.Random.Double(1, randomOptions.Value.MaxLength))
            .RuleFor(x => x.Weight,
                f => f.Random.Double(1, randomOptions.Value.MaxWeight));
    }

    private async Task SendMessages(IEnumerable<CalculationRequestMessage> messages,
        CancellationToken cancellationToken)
    {
        foreach (var message in messages)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _producer.ProduceAsync(
                _optionsSnapshot.Value.CalculationRequestTopicName,
                new Message<long, CalculationRequestMessage>
                {
                    Key = message.GoodId,
                    Value = message
                },
                cancellationToken);
        }
    }

    private CalculationRequestMessage[] GenerateMessages(int count)
    {
        lock (_lock)
        {
            return _faker.Generate(count).ToArray();
        }
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var command = await Task.Run(() =>
            {
                Console.Write("Input count of messages or \"exit\": ");
                return Console.ReadLine();
            }, cancellationToken);

            if (command == "exit")
            {
                break;
            }

            if (!int.TryParse(command, out var count))
            {
                Console.WriteLine("Invalid number");
                continue;
            }

            var messages = GenerateMessages(count);
            await SendMessages(messages, cancellationToken);

            Console.WriteLine("Messages sent");
        }
    }
}