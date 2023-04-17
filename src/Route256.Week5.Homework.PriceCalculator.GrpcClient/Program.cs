// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using Grpc.Core;
using Grpc.Net.Client;
using Route256.Week5.Homework.PriceCalculator.GrpcClient;

var channel = GrpcChannel.ForAddress("http://localhost:5001");
var client = new DeliveryPriceCalculator.DeliveryPriceCalculatorClient(channel);
if (client == null) throw new ArgumentNullException(nameof(client));

Dictionary<string, Func<Task>> availableMethods = new()
{
    {"Calculate", HandleCalculateCall},
    {"GetHistory", HandleGetHistoryCall},
    {"ClearHistory", HandleClearHistoryCall}
};

Console.WriteLine($"Доступные методы: {string.Join(", ", availableMethods)}");


Console.Write("Введите имя вызываемого метода: ");
var command = Console.ReadLine() ?? string.Empty;

while (!availableMethods.ContainsKey(command))
{
    Console.WriteLine("Такого метода не существует.");
    Console.Write("Введите имя вызываемого метода: ");
    command = Console.ReadLine() ?? string.Empty;
}

await availableMethods[command].Invoke();


Task HandleCalculateCall()
{
    var userId = GetUserIdInput();
    var goods = GetRepeatedGood();

    var request = new CalculationRequest
    {
        UserId = userId
    };
    request.Goods.AddRange(goods);

    var textRequest = JsonSerializer.Serialize(request);
    Console.WriteLine($"Метод Calculate вызван со следующими параметрами: {textRequest}");

    var result = client!.Calculate(request);
    
    Console.WriteLine($"CalculationResponse: {{ CalculationId: {result.CalculationId}, Price: {result.Price} }} ");
    
    return Task.CompletedTask;
}

async Task HandleGetHistoryCall()
{
    var userId = GetUserIdInput();

    var request = new GetHistoryRequest
    {
        UserId = userId
    };

    var stream = client.GetHistory(request);
    
    await foreach (var getHistoryResponse in stream.ResponseStream.ReadAllAsync())
    {
        var representation = JsonSerializer.Serialize<GetHistoryResponse>(getHistoryResponse);
        Console.WriteLine(representation);
    }
}

Task HandleClearHistoryCall()
{
    return Task.CompletedTask;
}

Good[] GetRepeatedGood()
{
    var goods = new List<Good>();
    const string inputMessage = "Для создания товара введите \"create\"." +
                                " Иначе, чтобы сформировать список введенных товаров, введите \"stop\": ";
    Console.Write(inputMessage);
    var line = Console.ReadLine();
    while (line != "stop")
    {
        var good = GetGood();
        goods.Add(good);

        Console.Write(inputMessage);
        line = Console.ReadLine();
    }

    return goods.ToArray();
}

Good GetGood()
{
    var height = GetGoodParameter("Height");
    var width = GetGoodParameter("Width");
    var length = GetGoodParameter("Length");
    var weight = GetGoodParameter("Weight");

    return new Good
    {
        Height = height,
        Length = length,
        Width = width,
        Weight = weight
    };
}

double GetGoodParameter(string parameterName)
{
    Console.Write($"Введите {parameterName}: ");

    double value;
    while (!double.TryParse(Console.ReadLine(), out value))
    {
        Console.WriteLine("Неверный формат числа.");
        Console.Write($"Введите {parameterName}: ");
    }

    return value;
}

long GetUserIdInput()
{
    Console.Write("Введите UserId: ");

    long value;
    while (!long.TryParse(Console.ReadLine(), out value))
    {
        Console.WriteLine("Неверный формат числа.");
    }

    return value;
}

Console.ReadKey();