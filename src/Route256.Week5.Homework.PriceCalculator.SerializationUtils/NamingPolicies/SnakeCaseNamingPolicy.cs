using System.Text.Json;
using Route256.Week6.Homework.PriceCalculator.SerializationUtlis.Extensions;

namespace Route256.Week6.Homework.PriceCalculator.SerializationUtlis.NamingPolicies;

public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) =>
            name.ToSnakeCase();
}