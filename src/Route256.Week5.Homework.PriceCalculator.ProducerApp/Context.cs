using Route256.Week5.Homework.PriceCalculator.ProducerApp.Interfaces;

namespace Route256.Week5.Homework.PriceCalculator.ProducerApp;

public class Context : IContext
{
    public string GetProjectDirectory()
    {
        return Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName ?? "";
    }
}
