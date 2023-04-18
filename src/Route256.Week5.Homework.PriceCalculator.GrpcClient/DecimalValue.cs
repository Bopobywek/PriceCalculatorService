namespace Route256.Week5.Homework.PriceCalculator.GrpcClient;

public partial class DecimalValue
{
    private const decimal NanoFactor = 1_000_000_000;
    
    public static decimal ToDecimal(DecimalValue decimalValue)
    {
        return decimalValue.units_ + decimalValue.nanos_ / NanoFactor;
    }

    public static DecimalValue FromDecimal(decimal standardDecimal)
    {
        var units = decimal.ToInt64(standardDecimal);
        var nanos = decimal.ToInt32((standardDecimal - units) * NanoFactor);

        return new DecimalValue
        {
            Units = units,
            Nanos = nanos
        };
    }
}