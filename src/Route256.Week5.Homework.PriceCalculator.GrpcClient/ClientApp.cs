using System.ComponentModel;
using System.Text.Json;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using Route256.Week5.Homework.PriceCalculator.GrpcClient.Interfaces;
using Route256.Week5.Homework.PriceCalculator.GrpcClient.Options;

namespace Route256.Week5.Homework.PriceCalculator.GrpcClient;

public class ClientApp : IConsoleApp
{
    private readonly DeliveryPriceCalculator.DeliveryPriceCalculatorClient _client;
    private readonly Dictionary<string, Func<Task>> _availableMethods;
    
    public ClientApp(IOptions<ClientOptions> options)
    {
        var channel = GrpcChannel.ForAddress(options.Value.ServiceEndpoint);
        _client = new DeliveryPriceCalculator.DeliveryPriceCalculatorClient(channel);
        _availableMethods = new Dictionary<string, Func<Task>>
        {
            {"Calculate", HandleCalculateCall},
            {"CalculateWithStreaming", HandleCalculateWithStreamingCall},
            {"GetHistory", HandleGetHistoryCall},
            {"ClearHistory", HandleClearHistoryCall}
        };
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        string exitCommand;
        do
        {
            Console.WriteLine($"Доступные методы: {string.Join(", ", _availableMethods.Keys)}");


            Console.Write("Введите имя вызываемого метода: ");
            var command = Console.ReadLine() ?? string.Empty;

            while (!_availableMethods.ContainsKey(command))
            {
                Console.WriteLine("Такого метода не существует.");
                Console.Write("Введите имя вызываемого метода: ");
                command = Console.ReadLine() ?? string.Empty;
            }

            await _availableMethods[command].Invoke();
            
            Console.WriteLine("Чтобы продолжить работу с программой нажмите \"Enter\"." +
                              " Для выхода из программы введите \"exit\"");
            exitCommand = Console.ReadLine() ?? string.Empty;
        } while (exitCommand != "exit");
    }

    private Task HandleCalculateWithStreamingCall()
    {
        return Task.CompletedTask;
    }

    private Task HandleCalculateCall()
    {
        var userId = GetParameter<long>("UserId");
        var goods = GetRepeated(GetGood);

        var request = new CalculationRequest
        {
            UserId = userId
        };
        request.Goods.AddRange(goods);

        var textRequest = JsonSerializer.Serialize(request);
        Console.WriteLine($"Метод Calculate вызван со следующими параметрами: {textRequest}");

        CalculationResponse result;
        try
        {
            result = _client.Calculate(request);
        }
        catch (RpcException exception)
        {
            Console.WriteLine($"Ошибка. Status: {exception.StatusCode}. Сообщение: {exception.Status.Detail}");
            return Task.CompletedTask;
        }

        Console.WriteLine(
            $"Результат вызова метода Calculate: {{ CalculationId: {result.CalculationId}," +
            $" Price: {DecimalValue.ToDecimal(result.Price)} }}\n");

        return Task.CompletedTask;
    }

    private async Task HandleGetHistoryCall()
    {
        var userId = GetParameter<long>("UserId");

        var request = new GetHistoryRequest
        {
            UserId = userId
        };

        try
        {
            var stream = _client.GetHistory(request);
            await foreach (var getHistoryResponse in stream.ResponseStream.ReadAllAsync())
            {
                var representation = JsonSerializer.Serialize(new
                {
                    getHistoryResponse.Cargo,
                    Price = DecimalValue.ToDecimal(getHistoryResponse.Price)
                });
                Console.WriteLine($"Получена запись: {representation}");
            }
            Console.WriteLine();
        }
        catch (RpcException exception)
        {
            Console.WriteLine($"Ошибка. Status: {exception.StatusCode}. Сообщение: {exception.Status.Detail}");
        }
    }

    private Task HandleClearHistoryCall()
    {
        var userId = GetParameter<long>("UserId");
        var calculationIds = GetRepeated(() => GetParameter<long>("CalculationId"));

        var request = new ClearHistoryRequest
        {
            UserId = userId,
        };
        request.CalculationIds.AddRange(calculationIds);

        try
        {
            _client.ClearHistory(request);
        }
        catch (RpcException exception)
        {
            Console.WriteLine($"Ошибка. Status: {exception.StatusCode}. Сообщение: {exception.Status.Detail}");
            return Task.CompletedTask;

        }

        Console.WriteLine("Запрос успешно выполнен.");

        return Task.CompletedTask;
    }

    private T[] GetRepeated<T>(Func<T> entityGetter)
    {
        var repeated = new List<T>();
        const string inputMessage = "Ввод нескольких объектов || Для создания объекта введите \"create\"." +
                                    "\nИначе, чтобы сформировать список введенных объектов, введите \"stop\": ";
        Console.Write(inputMessage);
        var line = Console.ReadLine();
        while (line != "stop")
        {
            T entity = entityGetter();
            repeated.Add(entity);
            Console.WriteLine("Объект добавлен к запросу.\n");

            Console.Write(inputMessage);
            line = Console.ReadLine();
        }

        return repeated.ToArray();
    }

    private Good GetGood()
    {
        var height = GetParameter<double>("Height");
        var width = GetParameter<double>("Width");
        var length = GetParameter<double>("Length");
        var weight = GetParameter<double>("Weight");

        return new Good
        {
            Height = height,
            Length = length,
            Width = width,
            Weight = weight
        };
    }

    private static bool TryParse<T>(string s, out T value) where T : struct
    {
        var converter = TypeDescriptor.GetConverter(typeof(T));
        try
        {
            value = (T) (converter.ConvertFromString(s) ?? throw new InvalidOperationException());
            return true;
        }
        catch
        {
            value = default;
            return false;
        }
    }

    private T GetParameter<T>(string parameterName) where T : struct
    {
        Console.Write($"Введите {parameterName}: ");

        T value;
        while (!TryParse(Console.ReadLine() ?? string.Empty, out value))
        {
            Console.WriteLine("Не удалось привести введенное значение к требуемому типу.");
            Console.Write($"Введите {parameterName}: ");
        }

        return value;
    }
}