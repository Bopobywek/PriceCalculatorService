namespace Route256.Week5.Homework.PriceCalculator.ProducerApp.Options;

public class ProducerAppOptions
{
    public string Broker { get; set; } = null!;
    public string CalculationRequestTopicName { get; set; } = null!;
}