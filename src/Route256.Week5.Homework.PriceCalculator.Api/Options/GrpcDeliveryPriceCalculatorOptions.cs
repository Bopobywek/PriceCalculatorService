namespace Route256.Week5.Homework.PriceCalculator.Api.Options;

public class GrpcDeliveryPriceCalculatorOptions
{
    public const string SectionName = "GrpcDeliveryPriceCalculatorOptions";
    public int HistoryTake { get; set; }
}