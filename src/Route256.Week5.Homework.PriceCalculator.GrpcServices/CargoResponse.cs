namespace Route256.Week5.Homework.PriceCalculator.GrpcServices;

public partial class CargoResponse
{
    public static CargoResponse Create(double volume, double weight, long[] goodIds)
    {
        var result = new CargoResponse
        {
            Volume = volume,
            Weight = volume
        };
        
        result.GoodIds.AddRange(goodIds);

        return result;
    }
}