namespace Route256.Week5.Homework.PriceCalculator.BackgroundServices.Options;

public class AnomalyFinderHostedServiceOptions
{
    public string Broker { get; set; } = null!;
    public string GroupId { get; set; } = null!;
    public string CalculationResultTopic { get; set; } = null!;
    public decimal NormalPriceThreshold { get; set; }
}