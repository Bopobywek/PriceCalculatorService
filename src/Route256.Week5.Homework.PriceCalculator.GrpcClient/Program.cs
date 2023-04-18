namespace Route256.Week5.Homework.PriceCalculator.GrpcClient;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var app = new ClientApp();
        await app.Run();
    }
}