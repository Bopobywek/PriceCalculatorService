namespace Route256.Week5.Homework.PriceCalculator.BackgroundServices.Options;

public class PriceCalculatorHostedServiceOptions
{
    public string Broker { get; set; } = null!;
    public string GroupId { get; set; } = null!;
    public string DeadLetterQueueTopic { get; set; } = null!;
    public string CalculationRequestTopic { get; set; } = null!;
    public string CalculationResultTopic { get; set; } = null!;
}