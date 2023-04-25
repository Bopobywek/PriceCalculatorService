using Route256.Week5.Homework.PriceCalculator.GrpcClient.Interfaces;

namespace Route256.Week5.Homework.PriceCalculator.GrpcClient;

public class Context : IContext
{
    public string GetProjectDirectory()
    {
        return Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName ?? "";
    }
}
